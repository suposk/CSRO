using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace CSRO.Server.Domain.AbstractValidation
{
    public class VmTicketValidator: AbstractValidator<VmTicketDto>
    {
        public VmTicketValidator()
        {
            RuleFor(p => p.VmName).NotEmpty();
            RuleFor(p => p.SubcriptionId).NotEmpty();
            RuleFor(p => p.ResorceGroup).NotEmpty();
        }
    }
}
