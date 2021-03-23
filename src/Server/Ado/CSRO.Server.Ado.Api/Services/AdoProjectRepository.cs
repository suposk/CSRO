using AutoMapper;
using CSRO.Common.AdoServices;
//using CSRO.Common.AdoServices.Models;
using AdoModels = CSRO.Common.AdoServices.Models;
using CSRO.Server.Entities;
using CSRO.Server.Entities.Entity;
using CSRO.Server.Infrastructure;
using CSRO.Server.Infrastructure.Search;
using CSRO.Server.Services.Utils;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
//using MediatR;
//using CSRO.Server.Ado.Api.Commands;
using CSRO.Server.Infrastructure.MessageBus;
using CSRO.Server.Ado.Api.Messaging;
using Microsoft.Extensions.Logging;

namespace CSRO.Server.Ado.Api.Services
{
    public interface IAdoProjectRepository : IRepository<AdoProject>
    {
        Task<List<AdoProject>> ApproveRejectAdoProjects(List<int> idList, bool reject);
        //Task<List<AdoProject>> ApproveAndCreateAdoProjects(List<int> toApprove);
        Task<AdoProject> CreateAdoProject(AdoProject entity);
        Task<bool> ProjectExists(string organization, string projectName, int projectId);
        Task<CsroPagedList<AdoProject>> Search(ResourceParameters resourceParameters, string organization = null);
    }

    public class AdoProjectRepository : Repository<AdoProject>, IAdoProjectRepository
    {        
        private readonly IRepository<AdoProject> _repository;
        private readonly AdoContext _context;        
        private readonly IAdoProjectHistoryRepository _adoProjectHistoryRepository;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly IMapper _mapper;
        private readonly ILogger<AdoProjectRepository> _logger;
        private readonly IMessageBus _messageBus;
        private string _userId;
        private readonly ServiceBusConfig _serviceBusConfig;

        public AdoProjectRepository(
            IConfiguration configuration,
            IRepository<AdoProject> repository,
            AdoContext context,                                    
            IAdoProjectHistoryRepository adoProjectHistoryRepository,
            IPropertyMappingService propertyMappingService, 
            IMapper mapper,
            ILogger<AdoProjectRepository> logger,
            IMessageBus messageBus,
            IApiIdentity apiIdentity) : base(context, apiIdentity)
        {            
            _repository = repository;
            _context = context;            
            _adoProjectHistoryRepository = adoProjectHistoryRepository;
            _propertyMappingService = propertyMappingService;
            _mapper = mapper;
            _logger = logger;
            _messageBus = messageBus;
            _userId = ApiIdentity.GetUserName();
            _serviceBusConfig = configuration.GetSection(nameof(ServiceBusConfig)).Get<ServiceBusConfig>();
        }

        public async Task<bool> ProjectExists(string organization, string projectName, int projectId)
        {
            if (string.IsNullOrWhiteSpace(projectName))            
                throw new ArgumentException($"'{nameof(projectName)}' cannot be null or whitespace.", nameof(projectName));            

            if (string.IsNullOrWhiteSpace(organization))            
                throw new ArgumentException($"'{nameof(organization)}' cannot be null or whitespace.", nameof(organization));

            //var res = await _context.AdoProjects.FirstOrDefaultAsync(a => a.IsDeleted != true &&
            //        a.Name.Equals(projectName, StringComparison.OrdinalIgnoreCase) && a.Organization.Equals(organization, StringComparison.OrdinalIgnoreCase));

            var que = _context.AdoProjects.Where(
                a => a.IsDeleted != true &&
                a.Name.ToLower() == projectName.ToLower() &&
                a.Organization.ToLower() == organization.ToLower());

            //may have by accisdet or bug multiple existing
            var res = await que.ToListAsync();
            bool exist = false;

            exist = res.HasAnyInCollection();
            if (!exist)
                return false;

            foreach (var pr in res)
            {
                //we may prform edit on existing record.
                if (pr.Id == projectId)
                    continue;

                if (pr.Status.ForbidenStatusForDuplicatePojectNames())
                    return true;
            }            
            return false;
        }

        public override Task<List<AdoProject>> GetList()
        {
            //return _repository.GetListFilter(a => a.IsDeleted != true);
            return _context.AdoProjects.Where(a => a.IsDeleted != true).OrderByDescending(a => a.CreatedAt).ToListAsync();
        }

        public async Task<AdoProject> CreateAdoProject(AdoProject entity)
        {
            try
            {
                Add(entity, _userId);
                entity.Status = Status.Submitted;
                if (!await SaveChangesAsync())
                    return null;
                await _adoProjectHistoryRepository.Create(entity.Id, IAdoProjectHistoryRepository.Operation_Request_Created, _userId);                
                return entity;
            }
            catch
            {
                throw;
            }           
        }
                
        public async Task<CsroPagedList<AdoProject>> Search(ResourceParameters resourceParameters, string organization = null)        
        {
            if (resourceParameters is null)
                throw new ArgumentNullException(nameof(resourceParameters));

            var collection = _context.AdoProjects as IQueryable<AdoProject>;

            if (!string.IsNullOrWhiteSpace(organization))
                collection = collection.Where(a => a.Organization == organization);

            if (resourceParameters.IsActive.HasValue)
                collection = collection.Where(a => a.IsDeleted == !resourceParameters.IsActive);

            //if (!string.IsNullOrWhiteSpace(resourceParameters.Type))
            //{
            //    var mainCategory = resourceParameters.Type.Trim();
            //    collection = collection.Where(a => a.MessageType.ToLower() == mainCategory.ToLower());
            //}

            if (!string.IsNullOrWhiteSpace(resourceParameters.SearchQuery))
            {

                var searchQuery = resourceParameters.SearchQuery.Trim();
                collection = collection.Where(a => a.Name.ToLower().Contains(searchQuery.ToLower()) || a.Organization.ToLower().Contains(searchQuery.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(resourceParameters.OrderBy))
            {
                var mapping = _propertyMappingService.GetPropertyMapping<AdoModels.ProjectAdo, AdoProject>();
                if (mapping != null)
                    collection = collection.ApplySort(resourceParameters.OrderBy, mapping);
            }
            else
                collection = collection.OrderBy(a => a.Id);

            var res = await CsroPagedList<AdoProject>.CreateAsync(collection,
                resourceParameters.PageNumber,
                resourceParameters.PageSize);
            
            return res;
        }

        public async Task<List<AdoProject>> ApproveRejectAdoProjects(List<int> idList, bool reject)
        {
            if (idList is null)
                throw new ArgumentNullException(nameof(idList));

            try
            {
                List<AdoProject> list = new();
                StringBuilder others = new();

                foreach (var pId in idList)
                {
                    try
                    {
                        var entity = await _repository.GetId(pId);
                        if (entity != null)
                        {
                            //only if in pending state
                            if (entity.State != ProjectState.CreatePending)
                                continue;
                            
                            var mapped = _mapper.Map<AdoModels.ProjectAdo>(entity);
                            //1. Update Db
                            //entity.Status = Status.Approved;                            
                            entity.Status = (reject) ? Status.Rejected : Status.Approved;
                            Update(entity, _userId);
                            if (await SaveChangesAsync())
                            {
                                if (reject)
                                    await _adoProjectHistoryRepository.Create(entity.Id, IAdoProjectHistoryRepository.Operation_Request_Rejected, _userId);                                    
                                else
                                    await _adoProjectHistoryRepository.Create(entity.Id, IAdoProjectHistoryRepository.Operation_Request_Approved, _userId);
                                list.Add(entity);
                            }
                        }
                        else
                            others.Append($"Id {pId} was not found, verify this Id exist or record was modified.");
                    }
                    catch (Exception ex)
                    {
                        others.Append($"Error approving Id {pId}: {ex.Message}");
                    }
                }

                BusMessageBase message = null;
                try
                {                    
                    if (reject)
                    {
                        message = new RejectedAdoProjectsMessage { RejectedAdoProjectIds = list.Select(a => a.Id).ToList(), UserId = _userId }.CreateBaseMessage();
                        await _messageBus.PublishMessageTopic(message, _serviceBusConfig.RejectedAdoProjectsTopic);
                    }
                    else
                    {
                        message = new ApprovedAdoProjectsMessage { ApprovedAdoProjectIds = list.Select(a => a.Id).ToList(), UserId = _userId }.CreateBaseMessage();
                        await _messageBus.PublishMessageTopic(message, _serviceBusConfig.ApprovedAdoProjectsTopic);
                        //await _messageBus.PublishMessageQueue(message, _serviceBusConfig.QueueNameTest);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Failed to PublishMessageTopic {ex?.Message}", message);
                }
                if (others.Length > 0)
                    _logger.LogWarning($"{nameof(ApproveRejectAdoProjects)} {others}", null);

                return list.Any() ? list : null;
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public override void Add(AdoProject entity, string UserId = null)
        {            
            base.Add(entity, _userId);
            entity.State = ProjectState.CreatePending;                        
        }

        public async override Task<AdoProject> UpdateAsync(AdoProject entity, string UserId = null)
        {
            if (await base.UpdateAsync(entity, _userId) != null)
            {
                await _adoProjectHistoryRepository.Create(entity.Id, IAdoProjectHistoryRepository.Operation_Request_Updated, _userId);
                return entity;
            }
            else 
                return null;
        }

        public override void Remove(AdoProject entity, string UserId = null)
        {
            base.Remove(entity, _userId);
            entity.State = ProjectState.Deleted;
            entity.Status = Status.Deleted;
        }
    }
}
