using CSRO.Server.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSRO.Server.Ado.Api.Services
{
    public interface IGenerateEmailForApprovalService
    {
        Task ApproveAdoProjects();
    }

    public class GenerateEmailForApprovalService : IGenerateEmailForApprovalService
    {
        private readonly IEmailService _emailService;
        private readonly IAdoProjectRepository _adoProjectRepository;
        private readonly ILogger<GenerateEmailForApprovalService> _logger;

        public GenerateEmailForApprovalService(
            IEmailService emailService,
            IAdoProjectRepository adoProjectRepository, 
            ILogger<GenerateEmailForApprovalService> logger
            )
        {
            _emailService = emailService;
            _adoProjectRepository = adoProjectRepository;
            _logger = logger;
        }

        public async Task ApproveAdoProjects()
        {
            try
            {                
                //var toApprove = await _adoProjectRepository.GetListFilter(a => a.State == Entities.Entity.ProjectState.CreatePending && (a.IsDeleted == null || a.IsDeleted == false)).ConfigureAwait(false);
                var toApprove = await _adoProjectRepository.GetListFilter(a => a.State == Entities.Entity.ProjectState.CreatePending && a.IsDeleted != true).ConfigureAwait(false);
                if (toApprove?.Count > 0)
                {
                    string text = $"There are {toApprove.Count} projects waiting for your approval.";
                    await _emailService.Send("jan.supolik@hotmail.com", "suposk@yahoo.com", $"test subject service at {DateTime.Now}", $"tested at {DateTime.Now}", false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(nameof(ApproveAdoProjects), ex);
            }            
        }
    }
}
