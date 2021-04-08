using CSRO.Server.Ado.Api.Dtos;
using FluentValidation;

namespace CSRO.Server.Ado.Api.Validation
{
    public class AdoProjectAccessModelValidator : AbstractValidator<AdoProjectAccessDto>
    {
        public AdoProjectAccessModelValidator()
        {
            RuleFor(p => p.Organization).NotEmpty();
            RuleFor(p => p.Name).NotEmpty();
            RuleFor(p => p.Justification).NotEmpty().MinimumLength(5);
        }
    }
}
