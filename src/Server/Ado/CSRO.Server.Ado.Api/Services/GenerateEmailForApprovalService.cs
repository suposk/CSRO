using CSRO.Server.Entities.Entity;
using CSRO.Server.Services.Ado;
using CSRO.Server.Services.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        private readonly IAdoProjectHistoryRepository _adoProjectHistoryRepository;
        private readonly IAdoProjectApproverService _adoProjectApproverService;
        private readonly ILogger<GenerateEmailForApprovalService> _logger;
        private readonly string _sender;

        public GenerateEmailForApprovalService(
            IEmailService emailService,            
            IAdoProjectHistoryRepository adoProjectHistoryRepository,
            IAdoProjectApproverService adoProjectApproverService,
            ILogger<GenerateEmailForApprovalService> logger
            )
        {
            _emailService = emailService;            
            _adoProjectHistoryRepository = adoProjectHistoryRepository;
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

                var toApprove = await _adoProjectHistoryRepository.GetPendingProjectsApproval();
                if (toApprove.IsNullOrEmptyCollection())
                {
                    _logger.LogDebug("No new project and email must be sent for approval");
                    return;
                }
                
                string text = $"There are {toApprove.Count} projects waiting for your approval.";
                long totalSent = 0;
                Parallel.ForEach<AdoProjectApprover, long>(
                    allApprovers, // source collection
                    () => 0,     // method to initialize the local variable
                    (approver, loopState, subtotal) => // method invoked by the loopState on each iteration
                    {
                       if (string.IsNullOrWhiteSpace(approver.Email))
                            return subtotal;

                        try
                        {                            
                            //_emailService.SendEmail(_sender, approver.Email, $"test subject service at {DateTime.Now}", $"tested at {DateTime.Now}", false).Wait();                                                
                            Task.Delay(5 * 1000).Wait(); //simulate sent email
                            subtotal++;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(nameof(ApproveAdoProjects), ex);
                        }                        
                        return subtotal;
                },
                // Method to be executed when each partition has completed.
                // finalResult is the final value of subtotal for a particular partition.
                (finalResult) => Interlocked.Add(ref totalSent, finalResult));

                //at least one email sent
                if (totalSent > 0)
                {
                    Parallel.ForEach(toApprove, async (project) =>
                    {
                        try
                        {
                            await _adoProjectHistoryRepository.Create(project.Id, IAdoProjectHistoryRepository.Operation_SentEmailForApproval, nameof(GenerateEmailForApprovalService));
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(nameof(ApproveAdoProjects), ex);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(nameof(ApproveAdoProjects), ex);
            }            
        }
    }
}
