using CSRO.Server.Ado.Api.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CSRO.Server.Ado.Api.BackgroundTasks
{
    /// <summary>
    /// call method to invoke project approval
    /// </summary>
    public class ProjectApprovalHostedService : BackgroundService
    {
        private readonly IGenerateEmailForApprovalService _generateEmailForApprovalService;
        private readonly ILogger<ProjectApprovalHostedService> _logger;

        public ProjectApprovalHostedService(
            IGenerateEmailForApprovalService generateEmailForApprovalService,
            ILogger<ProjectApprovalHostedService> logger)
        {
            _generateEmailForApprovalService = generateEmailForApprovalService;
            _logger = logger;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"${nameof(ProjectApprovalHostedService)} is starting.");
            await Task.Delay(10 * 1000).ConfigureAwait(false);            //initial delay

            stoppingToken.Register(() => _logger.LogInformation($"{nameof(ProjectApprovalHostedService)} register background task is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                int minutes = 1;
                _logger.LogDebug($"{nameof(ProjectApprovalHostedService)} background task is doing background work every {minutes} {nameof(minutes)}.");
                await _generateEmailForApprovalService.ApproveAdoProjects().ConfigureAwait(false);
                await Task.Delay(minutes * 60 * 1000, stoppingToken).ConfigureAwait(false);
            }
            _logger.LogInformation($"{nameof(ProjectApprovalHostedService)} background task is stopping.");

            await Task.CompletedTask;
        }
        
    }
}
