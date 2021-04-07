//using CSRO.Common.AdoServices.Models;
using AutoMapper;
using CSRO.Server.Entities;
using CSRO.Server.Entities.Entity;
using CSRO.Server.Infrastructure;
using CSRO.Server.Infrastructure.MessageBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CSRO.Server.Entities.Enums;
using System;
using CSRO.Server.Ado.Api.Messaging;
using System.Text;
//using MediatR;
//using CSRO.Server.Ado.Api.Commands;

namespace CSRO.Server.Ado.Api.Services
{
    public interface IAdoProjectAccessRepository: IRepository<AdoProjectAccess>
    {
        Task<List<AdoProjectAccess>> ApproveRejectAdoProjects(List<int> idList, bool reject, string reason);
        Task<AdoProjectAccess> CreateAdoProjectAccess(AdoProjectAccess entity);
    }

    public class AdoProjectAccessRepository : Repository<AdoProjectAccess>, IAdoProjectAccessRepository
    {
        private readonly IRepository<AdoProjectAccess> _repository;
        private readonly AdoContext _context;
        //private readonly IAdoProjectHistoryRepository _adoProjectHistoryRepository;        
        private readonly IMapper _mapper;
        private readonly ILogger<AdoProjectAccessRepository> _logger;
        private readonly IMessageBus _messageBus;
        private string _userId;
        private readonly ServiceBusConfig _serviceBusConfig;

        public AdoProjectAccessRepository(
            IConfiguration configuration,
            IRepository<AdoProjectAccess> repository,
            AdoContext context,
            //IAdoProjectHistoryRepository adoProjectHistoryRepository,            
            IMapper mapper,
            ILogger<AdoProjectAccessRepository> logger,
            IMessageBus messageBus,
            IApiIdentity apiIdentity) : base(context, apiIdentity)
        {
            _repository = repository;
            _context = context;
            //_adoProjectHistoryRepository = adoProjectHistoryRepository;            
            _mapper = mapper;
            _logger = logger;
            _messageBus = messageBus;
            _userId = ApiIdentity.GetUserName();
            _serviceBusConfig = configuration.GetSection(nameof(ServiceBusConfig)).Get<ServiceBusConfig>();
        }

        public override Task<List<AdoProjectAccess>> GetList()
        {
            return _context.AdoProjectAccesses.Where(a => a.IsDeleted != true).OrderByDescending(a => a.CreatedAt).ToListAsync();
        }

        public async Task<AdoProjectAccess> CreateAdoProjectAccess(AdoProjectAccess entity)
        {
            try
            {
                Add(entity, _userId);
                entity.Status = Status.Submitted;
                if (!await SaveChangesAsync())
                    return null;
                //await _adoProjectHistoryRepository.Create(entity.Id, IAdoProjectHistoryRepository.Operation_Request_Created, _userId);
                return entity;
            }
            catch
            {
                throw;
            }
        }

        public async Task<List<AdoProjectAccess>> ApproveRejectAdoProjects(List<int> idList, bool reject, string reason)
        {
            if (idList is null)
                throw new ArgumentNullException(nameof(idList));

            try
            {
                List<AdoProjectAccess> list = new();
                StringBuilder others = new();

                foreach (var pId in idList)
                {
                    try
                    {
                        var entity = await _repository.GetId(pId);
                        if (entity != null)
                        {
                            //only if in pending state
                            if (entity.Status != Status.Submitted)
                                continue;

                            //1. Update Db
                            //entity.Status = Status.Approved;                            
                            entity.Status = (reject) ? Status.Rejected : Status.Approved;
                            Update(entity, _userId);
                            if (await SaveChangesAsync())
                            {
                                //if (reject)
                                //    await _adoProjectHistoryRepository.Create(entity.Id, IAdoProjectHistoryRepository.Operation_Request_Rejected, _userId, reason);
                                //else
                                //    await _adoProjectHistoryRepository.Create(entity.Id, IAdoProjectHistoryRepository.Operation_Request_Approved, _userId, reason);
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
                    //if (reject)
                    //{
                    //    message = new RejectedAdoProjectsMessage { RejectedAdoProjectIds = list.Select(a => a.Id).ToList(), UserId = _userId, Reason = reason }.CreateBaseMessage();
                    //    await _messageBus.PublishMessageTopic(message, _serviceBusConfig.RejectedAdoProjectsTopic);
                    //}
                    //else
                    //{
                    //    message = new ApprovedAdoProjectsMessage { ApprovedAdoProjectIds = list.Select(a => a.Id).ToList(), UserId = _userId }.CreateBaseMessage();
                    //    await _messageBus.PublishMessageTopic(message, _serviceBusConfig.ApprovedAdoProjectsTopic);
                    //    //await _messageBus.PublishMessageQueue(message, _serviceBusConfig.QueueNameTest);
                    //}
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Failed to PublishMessageTopic {ex?.Message}", message);
                }
                if (others.Length > 0)
                    _logger.LogWarning($"{nameof(ApproveRejectAdoProjects)} {others}", null);

                return list.Any() ? list : null;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public override void Remove(AdoProjectAccess entity, string UserId = null)
        {
            base.Remove(entity, _userId);
            entity.Status = Status.Deleted;
        }
    }
}
