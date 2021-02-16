using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace CSRO.Server.Domain.AbstractValidation
{
    public class VmTicketValidator: AbstractValidator<VmTicketDto>
    {
        public VmTicketValidator()
        {
            RuleFor(p => p.VmName).NotEmpty();
            RuleFor(p => p.SubcriptionId).NotEmpty();
            RuleFor(p => p.ResorceGroup).NotEmpty();

            When(p => string.IsNullOrWhiteSpace(p.SubcriptionName), () =>
            {
                RuleFor(p => p.SubcriptionName)
                .NotEmpty()
                .WithMessage("Subcription Name must be selected")
                ;
            }).Otherwise(() =>
            {
                RuleFor(p => p.SubcriptionName)
                    //.Must((p,t) => p.SubcriptionName.Contains("dev", StringComparison.OrdinalIgnoreCase))
                    //.Must((p,t) => p.SubcriptionName.ToLower().Contains("dev"))
                    .Must((p) => p.ToLower().Contains("dev")) //todo fix
                    .WithMessage("Only dev, appdev Subscripion")
                    ;
            });

            When(p => string.IsNullOrWhiteSpace(p.ExternalTicket), () =>
            {
                RuleFor(p => p.ExternalTicket).NotEmpty()
                .WithMessage("Ticket # must be entered");
            }).Otherwise(() =>
            {
                RuleFor(p => p.ExternalTicket)
                    .Must(p => p.StartsWith("inc", StringComparison.OrdinalIgnoreCase))
                    .WithMessage("Ticket # must start with INC. Only Gsnow incident tickets.")
                    .MinimumLength(8)
                ;
            });
        }
    }
}
