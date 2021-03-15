using CSRO.Common.AdoServices.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Common.AdoServices.Validation
{
    public class ProjectAdoValidator : AbstractValidator<ProjectAdo>
    {
        public ProjectAdoValidator()
        {            
            RuleFor(p => p.Organization).NotEmpty();
            RuleFor(p => p.ProcessName).NotEmpty();
            RuleFor(p => p.Name).NotEmpty();
        }
    }
}
