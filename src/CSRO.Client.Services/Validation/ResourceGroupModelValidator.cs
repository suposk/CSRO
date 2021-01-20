using CSRO.Client.Services.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSRO.Client.Services.Validation
{
    public class ResourceGroupValidator : AbstractValidator<ResourceGroup>
    {
        public ResourceGroupValidator()
        {
            RuleFor(p => p.Name).NotEmpty()
                .WithMessage("Resource Group must be selected");

            RuleFor(p => p.Location).NotEmpty()
                .WithMessage("Location must be selected");
        }
    }

    public class ResourceGroupModelValidator : AbstractValidator<ResourceGroupModel>
    {
        public ResourceGroupModelValidator(
            //ISubcriptionService subcriptionService 
            )
        {
            //RuleFor(p => p.SubcriptionName)
            //    .NotEmpty().WithMessage("Subscripion must be selected");

            RuleFor(p => p.SubscripionIdName).SetValidator(new SubscripionIdNameValidator());

            RuleFor(p => p.ResourceGroup).SetValidator(new ResourceGroupValidator());                   
        }
    }
}
