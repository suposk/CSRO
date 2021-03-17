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
            _projectAdoServices = projectAdoServices;

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
        }

        private async Task<bool> ProjectName(string name, CancellationToken token)
        {
            if (token.IsCancellationRequested)
                //return Task.FromResult(false);
                return false;

            if (string.IsNullOrWhiteSpace(name))
                //return Task.FromResult(false);
                return false;

            var exisit = await _projectAdoServices.ProjectExistInAdo("jansupolikAdo", name);
            return !exisit;

            //return _projectAdoServices.ProjectDoesNotExistInAdo("jansupolikAdo", name);            
        }

        private async Task<bool> ProjecRequested(string name, CancellationToken token)
        {           
            if (token.IsCancellationRequested)
                return false;

            if (string.IsNullOrWhiteSpace(name))
                return false;

            //return false;
            return true;
        }
    }
}
