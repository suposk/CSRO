using CSRO.Client.Services.Models;
using FluentValidation;

namespace CSRO.Client.Services.Validation
{
    public class DefaultTagValidator : AbstractValidator<DefaultTag>
    {
        public DefaultTagValidator()
        {
            RuleFor(p => p.billingReference).NotEmpty();
            RuleFor(p => p.cmdbRerence).NotEmpty();
            RuleFor(p => p.opEnvironment).NotEmpty();
        }
    }
}
