using CSRO.Client.Services.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Client.Services.Validation
{

    public class VmTicketValidator : AbstractValidator<VmTicket>
    {
        public VmTicketValidator()
        {
            RuleFor(p => p.VmName).NotEmpty();
            RuleFor(p => p.SubcriptionId).NotEmpty();
            RuleFor(p => p.ResorceGroup).NotEmpty();
        }
    }
}
