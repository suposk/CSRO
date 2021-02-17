using CSRO.Common.AzureSdkServices;
using CSRO.Server.Domain;
using FluentValidation;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSRO.Server.Services.Validation
{
    public class VmTicketValidator : AbstractValidator<VmTicketDto>
    {
        private readonly IGsnowService _gsnowService;
        private readonly ILogger<VmTicketValidator> _logger;

        public VmTicketValidator(IGsnowService gsnowService, ILogger<VmTicketValidator> logger)
        {
            _gsnowService = gsnowService;
            _logger = logger;

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
                    .MustAsync(ValidateGsnow)
                    .WithMessage("Invalid Gsnow Ticket")                    
                ;
            });            

        }

        private async Task<bool> ValidateGsnow(string gsnowTicket, CancellationToken cancelToken)
        {
            //gsnowTicket = null; //test
            try
            {
                var valid = await _gsnowService.IsValidGsnowTicket(gsnowTicket, cancelToken);
                return valid;
            }
            catch (Exception ex)
            {
                _logger.LogError(nameof(ValidateGsnow), ex);
            }
            return false;
        }
    }
}
