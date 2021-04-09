using FluentValidation;

namespace CSRO.Client.Services.Validation
{
    public class PrivMembersTagValidator : DefaultTagValidator
    {
        public PrivMembersTagValidator()
        {
            RuleFor(p => p.privilegedMembers).NotEmpty();
        }
    }

}
