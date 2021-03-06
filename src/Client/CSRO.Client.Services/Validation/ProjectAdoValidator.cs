﻿using CSRO.Common.AdoServices;
using CSRO.Common.AdoServices.Models;
using FluentValidation;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSRO.Client.Services.Validation
{
    public class ProjectAdoValidator: AbstractValidator<ProjectAdo>
    {
        private readonly IProjectAdoServices _projectAdoServices;
        private readonly IAdoProjectDataService _adoProjectDataService;
        private readonly ILogger<ProjectAdoValidator> _logger;

        public ProjectAdoValidator(IProjectAdoServices projectAdoServices, IAdoProjectDataService adoProjectDataService, ILogger<ProjectAdoValidator> logger)
        {
            _projectAdoServices = projectAdoServices;
            _adoProjectDataService = adoProjectDataService;
            _logger = logger;

            RuleFor(p => p.Organization)
            .NotEmpty()
            .WithMessage("Organization must be selected");

            RuleFor(p => p.ProcessName)
            .NotEmpty()
            .WithMessage("Process must be selected");

            When(p => string.IsNullOrWhiteSpace(p.Name), () =>
            {
                RuleFor(p => p.Name).NotEmpty()
                .WithMessage("Name must be entered");
            }).Otherwise(() =>
            {
                RuleFor(p => p.Name)
                .MustAsync(ProjectName).WithMessage("Project already exist in ADO, Enter different name")
                .MustAsync(ProjecAlredyRequested).WithMessage("Project already requested, Enter different name")
                ;
            });            
        }

        private async Task<bool> ProjectName(ProjectAdo projectAdo, string name, CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return false;

            if (projectAdo == null || string.IsNullOrWhiteSpace(projectAdo.Name) || string.IsNullOrWhiteSpace(projectAdo.Organization))
                return false;
            try
            {
                var exisit = await _projectAdoServices.ProjectExistInAdo(projectAdo.Organization, projectAdo.Name);
                return !exisit;
            }
            catch (Exception ex) { _logger.LogError(ex, nameof(ProjectName), projectAdo); }
            return false;
        }

        private async Task<bool> ProjecAlredyRequested(ProjectAdo projectAdo, string name, CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return false;

            if (projectAdo == null || string.IsNullOrWhiteSpace(projectAdo.Name) || string.IsNullOrWhiteSpace(projectAdo.Organization))
                return false;

            try
            {
                var exisit = await _adoProjectDataService.ProjectExists(projectAdo.Organization, projectAdo.Name, projectAdo.Id);
                return !exisit;
            }
            catch (Exception ex) { _logger.LogError(ex, nameof(ProjecAlredyRequested), projectAdo); }
            return false;
        }
    }
}
