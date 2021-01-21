using CSRO.Client.Services.Models;
using FluentValidation;

namespace CSRO.Client.Services.Validation
{
    public class LocationIdNameValidator : AbstractValidator<IdName>
    {
        public LocationIdNameValidator()
        {
            RuleFor(p => p.Id).NotEmpty()
                .WithMessage("Location Val must be selected");

            RuleFor(p => p.Name).NotEmpty()
                .WithMessage("Location Val must be selected");

        }
    }
}
