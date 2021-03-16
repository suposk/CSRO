using CSRO.Server.Services.Ado;
using CSRO.Server.Services.Utils;
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
        private readonly IAdoProjectApproverService _adoProjectApproverService;
        private readonly ILogger<GenerateEmailForApprovalService> _logger;
        private readonly string _sender;

        public GenerateEmailForApprovalService(
            IEmailService emailService,
            IAdoProjectRepository adoProjectRepository,
            IAdoProjectApproverService adoProjectApproverService,
            ILogger<GenerateEmailForApprovalService> logger
            )
        {
            _emailService = emailService;
            _adoProjectRepository = adoProjectRepository;
            _adoProjectApproverService = adoProjectApproverService;
            _logger = logger;

            //TODO Config value
            _sender = "jan.supolik@hotmail.com";
        }

        public async Task ApproveAdoProjects()
        {
            try
            {
                var allApprovers = await _adoProjectApproverService.GetAdoProjectApprovers();
                //if (allApprovers?.Any() == false)
                if (allApprovers.IsNullOrEmptyCollection())
                    return;                               
                                
                var toApprove = await _adoProjectRepository.GetListFilter(a => a.State == Entities.Entity.ProjectState.CreatePending && a.IsDeleted != true).ConfigureAwait(false);
                if (toApprove.IsNullOrEmptyCollection())
                    return;
                
                string text = $"There are {toApprove.Count} projects waiting for your approval.";
                Parallel.ForEach(allApprovers, async (approver) => 
                {
                    if (string.IsNullOrWhiteSpace(approver.Email))
                        return;

                    try
                    {
                        //await _emailService.SendEmail(_sender, approver.Email, $"test subject service at {DateTime.Now}", $"tested at {DateTime.Now}", false);                        
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(nameof(ApproveAdoProjects), ex);
                    }
                    int x = 1;
                });
                //await _emailService.SendEmail(_sender, "suposk@yahoo.com", $"test subject service at {DateTime.Now}", $"tested at {DateTime.Now}", false);
                
            }
            catch (Exception ex)
            {
                _logger.LogError(nameof(ApproveAdoProjects), ex);
            }            
        }
    }
}
