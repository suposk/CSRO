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

namespace CSRO.Server.Ado.Api.Services
{
    public interface IAdoProjectRepository : IRepository<AdoProject>
    {
        Task<List<AdoProject>> ApproveAdoProject(List<int> toApprove);
        Task<AdoProject> CreateAdoProject(AdoProject entity);
        Task<bool> ProjectExists(string organization, string projectName);
        Task<CsroPagedList<AdoProject>> Search(ResourceParameters resourceParameters, string organization = null);
    }

    public class AdoProjectRepository : Repository<AdoProject>, IAdoProjectRepository
    {        
        private readonly IRepository<AdoProject> _repository;
        private readonly AdoContext _context;
        private readonly IProjectAdoServices _projectAdoServices;
        private readonly IAdoProjectHistoryRepository _adoProjectHistoryRepository;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly IMapper _mapper;
        private string _userId;        

        public AdoProjectRepository(            
            IRepository<AdoProject> repository,
            AdoContext context,                        
            IProjectAdoServices projectAdoServices,
            IAdoProjectHistoryRepository adoProjectHistoryRepository,
            IPropertyMappingService propertyMappingService, 
            IMapper mapper,
            IApiIdentity apiIdentity) : base(context, apiIdentity)
        {            
            _repository = repository;
            _context = context;
            _projectAdoServices = projectAdoServices;
            _adoProjectHistoryRepository = adoProjectHistoryRepository;
            _propertyMappingService = propertyMappingService;
            _mapper = mapper;
            _userId = ApiIdentity.GetUserName();            
        }

        public async Task<bool> ProjectExists(string organization, string projectName)
        {
            if (string.IsNullOrWhiteSpace(projectName))            
                throw new ArgumentException($"'{nameof(projectName)}' cannot be null or whitespace.", nameof(projectName));            

            if (string.IsNullOrWhiteSpace(organization))            
                throw new ArgumentException($"'{nameof(organization)}' cannot be null or whitespace.", nameof(organization));

            //var res = await _context.AdoProjects.FirstOrDefaultAsync(a => a.IsDeleted != true &&
            //        a.Name.Equals(projectName, StringComparison.OrdinalIgnoreCase) && a.Organization.Equals(organization, StringComparison.OrdinalIgnoreCase));

            var res = await _context.AdoProjects.FirstOrDefaultAsync(a => a.IsDeleted != true &&
                    a.Name.ToLower() == projectName.ToLower() && a.Organization.ToLower() == organization.ToLower());

            return res != null;
        }

        public override Task<List<AdoProject>> GetList()
        {
            return _repository.GetListFilter(a => a.IsDeleted != true);
        }

        public async Task<AdoProject> CreateAdoProject(AdoProject entity)
        {
            try
            {
                Add(entity, _userId);
                entity.Status = Status.Submitted;
                if (!await SaveChangesAsync())
                    return null;
                await _adoProjectHistoryRepository.Create(entity.Id, IAdoProjectHistoryRepository.Operation_RequestCreated, _userId);                
                return entity;
            }
            catch
            {
                throw;
            }           
        }

        //public async Task<CsroPagedList<AdoProject>> Search(ResourceParameters resourceParameters)
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

        public async Task<List<AdoProject>> ApproveAdoProject(List<int> toApprove)
        {
            if (toApprove is null)
                throw new ArgumentNullException(nameof(toApprove));

            try
            {                         
                List<AdoProject> approved = new();
                StringBuilder others = new();

                foreach (var pId in toApprove)
                {

                    try
                    {
                        var entity = await _repository.GetId(pId);
                        if (entity != null)
                        {
                            //only if in pending state
                            if (entity.State != ProjectState.CreatePending)
                                continue;

                            //1. create Proj
                            var mapped = _mapper.Map<AdoModels.ProjectAdo>(entity);                           
                            var created = await _projectAdoServices.CreateProject(mapped);

                            //2. Update Db
                            entity = _mapper.Map<AdoProject>(created);
                            //entity.State = ProjectState.New;
                            base.Update(entity, _userId);
                            if (await SaveChangesAsync())
                            {
                                await _adoProjectHistoryRepository.Create(entity.Id, IAdoProjectHistoryRepository.Operation_RequestApproved, _userId);
                                approved.Add(entity);
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
                return approved.Any() ? approved : null;
            }
            catch
            {
                throw;
            }            
        }

        public override void Add(AdoProject entity, string UserId = null)
        {            
            base.Add(entity, _userId);
            entity.State = ProjectState.CreatePending;                        
        }

        public override void Remove(AdoProject entity, string UserId = null)
        {
            base.Remove(entity, _userId);
            entity.State = ProjectState.Deleted;
            entity.Status = Status.Deleted;
        }
    }
}
