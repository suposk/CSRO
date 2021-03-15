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
        private readonly ILogger<ProjectApprovalHostedService> _logger;

        public ProjectApprovalHostedService(ILogger<ProjectApprovalHostedService> logger)
        {
            _logger = logger;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"${nameof(ProjectApprovalHostedService)} is starting.");
            await Task.Delay(10 * 1000);            //initial delay

            stoppingToken.Register(() => _logger.LogInformation($"${nameof(ProjectApprovalHostedService)} register background task is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                int minutes = 1;
                _logger.LogDebug($"${nameof(ProjectApprovalHostedService)} background task is doing background work every {minutes} {nameof(minutes)}.");

                await Task.Delay(minutes * 60 * 1000, stoppingToken);
            }
            _logger.LogInformation($"${nameof(ProjectApprovalHostedService)} background task is stopping.");

            await Task.CompletedTask;
        }
        
    }
}
