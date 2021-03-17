using CSRO.Common.AdoServices;
using CSRO.Common.AdoServices.Models;
using FluentValidation;
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

        public ProjectAdoValidator(IProjectAdoServices projectAdoServices)
        {
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
                .MustAsync(ProjecRequested).WithMessage("Project already requested, Enter different name")
                ;
            });
            _projectAdoServices = projectAdoServices;
        }

        private async Task<bool> ProjectName(string name, CancellationToken token)
        {
            await Task.Delay(1 * 300, token).ConfigureAwait(false);
            if (token.IsCancellationRequested)
                return false;

            if (name == null)
                return false;

            var exisit = await _projectAdoServices.ProjectExistInAdo("jansupolikAdo", name).ConfigureAwait(false);
            return !exisit;
        }

        private async Task<bool> ProjecRequested(string name, CancellationToken token)
        {
            await Task.Delay(1 * 300, token).ConfigureAwait(false);
            if (token.IsCancellationRequested)
                return false;

            if (name == null)
                return false;

            return false;
        }
    }
}
