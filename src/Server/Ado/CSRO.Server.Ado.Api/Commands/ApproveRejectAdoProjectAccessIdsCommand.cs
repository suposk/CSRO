using AutoMapper;
using CSRO.Common.AdoServices;
using CSRO.Server.Ado.Api.Services;
using CSRO.Server.Entities.Entity;
using CSRO.Server.Entities.Enums;
using CSRO.Server.Infrastructure;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AdoModels = CSRO.Common.AdoServices.Models;

namespace CSRO.Server.Ado.Api.Commands
{
    public class ApproveRejectAdoProjectAccessIdsCommand : IRequest<List<AdoProjectAccess>>
    {
        public List<int> IdsList { get; set; }
        public bool Reject { get; set; }
        public string Reason { get; set; }
        //public string UserId { get; set; }
    }

    public class ApproveRejectAdoProjectAccessIdsCommandHandler : IRequestHandler<ApproveRejectAdoProjectAccessIdsCommand, List<AdoProjectAccess>>
    {
        private readonly IProjectAdoServices _projectAdoServices;
        private readonly IAdoProjectAccessRepository _adoProjectAccessRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ApproveRejectAdoProjectAccessIdsCommandHandler> _logger;
        private readonly string _userId;

        public ApproveRejectAdoProjectAccessIdsCommandHandler
            (
            IProjectAdoServices projectAdoServices,
            IAdoProjectAccessRepository adoProjectAccessRepository,
            IMapper mapper,
            IApiIdentity apiIdentity,
            ILogger<ApproveRejectAdoProjectAccessIdsCommandHandler> logger
            )
        {
            _projectAdoServices = projectAdoServices;
            _adoProjectAccessRepository = adoProjectAccessRepository;
            _mapper = mapper;
            _userId = apiIdentity.GetUserName();
            _logger = logger;
            //com 5 06
        }

        public async Task<List<AdoProjectAccess>> Handle(ApproveRejectAdoProjectAccessIdsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _adoProjectAccessRepository.DatabaseContext.ChangeTracker.Clear();

                List<AdoProjectAccess> created = new();
                StringBuilder warnings = new();
                if (request.IdsList.IsNullOrEmptyCollection())
                    return null;

                AdoProjectAccess origEntity = null;
                foreach (var id in request.IdsList)
                {
                    try
                    {
                        origEntity = null;
                        origEntity = await _adoProjectAccessRepository.GetId(id);

                        if (request.Reject)
                        {
                            //2. Update Db                            
                            origEntity.Status = Status.Rejected;
                            created.Add(origEntity);
                        }
                        else
                        {
                            //1. create Proj
                            //var mapped = _mapper.Map<AdoModels.ProjectAdo>(origEntity);
                            var sec = await _projectAdoServices.AddUser(origEntity.Organization, origEntity.Name, origEntity.CreatedBy);

                            //2. Update Db                            
                            origEntity.Status = Status.Completed;
                            created.Add(origEntity);
                        }

                        _adoProjectAccessRepository.Update(origEntity, _userId);
                        if (await _adoProjectAccessRepository.SaveChangesAsync() && origEntity.Status == Status.Completed)
                        {
                            //3. create record in history DB                            
                            //await _adoProjectHistoryRepository.Create(origEntity.Id, IAdoProjectHistoryRepository.Operation_Request_Completed, request.UserId);

                            //4. Send email
                        }
                    }
                    catch (Exception ex)
                    {
                        warnings.Append($"Error approving Id: {origEntity?.Id}, {nameof(origEntity.Name)}: {origEntity?.Name}, {ex.Message}");
                    }
                }
                if (warnings.Length > 0)
                    _logger.LogWarning($"{nameof(CreateApprovedAdoProjectIdsCommandHandler)} warnings to report: {warnings}");

                return created;
            }
            catch (Exception ex)
            {
                //this shouldn't stop the API from doing else so this can be logged
                _logger.LogError($"{nameof(CreateApprovedAdoProjectIdsCommandHandler)} failed due to: {ex.Message}");
            }
            return null;
        }
    }
}
