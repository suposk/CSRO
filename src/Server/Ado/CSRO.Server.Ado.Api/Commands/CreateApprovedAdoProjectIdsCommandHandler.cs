using AutoMapper;
using CSRO.Common.AdoServices;
using CSRO.Server.Ado.Api.Services;
using CSRO.Server.Entities.Entity;
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
    public class CreateApprovedAdoProjectIdsCommandHandler : IRequestHandler<CreateApprovedAdoProjectIdsCommand, List<AdoProject>>
    {
        private readonly IProjectAdoServices _projectAdoServices;
        private readonly IAdoProjectRepository _adoProjectRepository;
        private readonly IAdoProjectHistoryRepository _adoProjectHistoryRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateApprovedAdoProjectIdsCommandHandler> _logger;

        public CreateApprovedAdoProjectIdsCommandHandler
            (
            IProjectAdoServices projectAdoServices,
            IAdoProjectRepository adoProjectRepository,
            IAdoProjectHistoryRepository adoProjectHistoryRepository,
            IMapper mapper,
            ILogger<CreateApprovedAdoProjectIdsCommandHandler> logger
            )
        {
            _projectAdoServices = projectAdoServices;
            _adoProjectRepository = adoProjectRepository;
            _adoProjectHistoryRepository = adoProjectHistoryRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<AdoProject>> Handle(CreateApprovedAdoProjectIdsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _adoProjectRepository.DatabaseContext.ChangeTracker.Clear();
                _adoProjectHistoryRepository.DatabaseContext.ChangeTracker.Clear();

                List<AdoProject> created = new();
                StringBuilder warnings = new();
                if (request.Approved.IsNullOrEmptyCollection())
                    return null;
                AdoProject origEntity = null;
                foreach (var id in request.Approved)
                {
                    try
                    {
                        origEntity = null;
                        origEntity = await _adoProjectRepository.GetId(id);

                        //1. create Proj
                        var mapped = _mapper.Map<AdoModels.ProjectAdo>(origEntity);
                        var createdModel = await _projectAdoServices.CreateProject(mapped);

                        //2. Update Db
                        var createdEntity = _mapper.Map<AdoProject>(createdModel);
                        createdEntity.Status = origEntity.Status;

                        if (createdModel.State == Microsoft.TeamFoundation.Core.WebApi.ProjectState.New ||
                            createdModel.State == Microsoft.TeamFoundation.Core.WebApi.ProjectState.WellFormed)
                        {
                            createdEntity.Status = Status.Completed;
                            created.Add(createdEntity);
                        }

                        _adoProjectRepository.Update(createdEntity, request.UserId);
                        if (await _adoProjectRepository.SaveChangesAsync() && createdEntity.Status == Status.Completed)
                        {
                            //3. create record in history DB                            
                            await _adoProjectHistoryRepository.Create(origEntity.Id, IAdoProjectHistoryRepository.Operation_Request_Completed, request.UserId);
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
