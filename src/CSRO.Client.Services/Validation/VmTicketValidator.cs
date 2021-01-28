using CSRO.Client.Core.Models;
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
    public class SubscripionIdNameValidator : AbstractValidator<IdName>
    {
        public SubscripionIdNameValidator()
        {
            RuleFor(p => p.Name).NotEmpty()
                .WithMessage("Subscripion must be selected");
        }
    }

    public class VmTicketValidator : AbstractValidator<VmTicket>
    {
        public VmTicketValidator(
            //ISubcriptionService subcriptionService 
            )
        {
            //RuleFor(p => p.SubcriptionName)
            //    .NotEmpty().WithMessage("Subscripion must be selected");

            RuleFor(p => p.SubscripionIdName).SetValidator(new SubscripionIdNameValidator());

            RuleFor(p => p.ResorceGroup).NotEmpty();

            RuleFor(p => p.VmName)
                .NotEmpty()
                //.MustAsync(ValidateVm).WithMessage("Test only, name has to contain vm letter")
                ;

            //RuleFor(p => p.SubcriptionId)
            //    .NotEmpty()
            //    //.MustAsync(subcriptionService.SubcriptionExist).WithMessage("Subcription Id not found");
            //;                       
        }

        private async Task<bool> ValidateVm(string vm, CancellationToken token)
        {            
            await Task.Delay(1 * 1000, token).ConfigureAwait(false);
            if (token.IsCancellationRequested)
                return false;

            if (vm == null)
                return false;
            return vm.ToLower().Contains("vm") ? true : false;
        }
    }
}
