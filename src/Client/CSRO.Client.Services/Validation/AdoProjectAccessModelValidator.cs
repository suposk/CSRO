using CSRO.Client.Services.Models;
using FluentValidation;

namespace CSRO.Client.Services.Validation
{
    public class AdoProjectAccessModelValidator : AbstractValidator<AdoProjectAccessModel>
    {
        public AdoProjectAccessModelValidator()
        {
            RuleFor(p => p.Organization).NotEmpty();
            RuleFor(p => p.Name).NotEmpty();
            RuleFor(p => p.Justification).NotEmpty().MinimumLength(5);
        }
    }
}
