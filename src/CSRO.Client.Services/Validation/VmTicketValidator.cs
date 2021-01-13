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

    public class VmTicketValidator : AbstractValidator<VmTicket>
    {
        public VmTicketValidator()
        {
            RuleFor(p => p.VmName)
                .NotEmpty()
                .MustAsync(ValidateVm).WithMessage("Test only, name has to contain vm letter");                

            RuleFor(p => p.SubcriptionId).NotEmpty();
            RuleFor(p => p.ResorceGroup).NotEmpty();
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
