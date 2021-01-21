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
                .WithMessage("Resource Group must be selected")
                //.Matches(@"^[-\w\._\(\)]+$").WithMessage("Invalid Characters")
                ;

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
            RuleFor(p => p.SubscripionIdName).SetValidator(new SubscripionIdNameValidator());                       

            When(p => p.IsNewRg, () => 
            {
                RuleFor(p => p.NewRgName)
                    .NotEmpty().WithMessage("Resource Group must be entered")
                    .Matches(@"^[-\w\._\(\)]+$").WithMessage("Resource group names only allow alphanumeric characters, periods, underscores, hyphens and parenthesis and cannot end in a period.");

                RuleFor(p => p.Location).NotEmpty()
                .WithMessage("Location must be selected");
            }).Otherwise(() => 
            {
                RuleFor(p => p.ResourceGroup).SetValidator(new ResourceGroupValidator());
            });

            //RuleFor(p => p.LocationIdName).SetValidator(new LocationIdNameValidator());

            //RuleFor(p => p.Location)
            //    .NotEmpty().WithMessage("Location WRAPPER must be selected");
        }
    }
}
