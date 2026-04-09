using CES.BackendService.Contracts;
using FluentValidation;

namespace CES.BackendService.Validation;

public class ForumRegistrationRequestValidator : AbstractValidator<ForumRegistrationRequest>
{
    public ForumRegistrationRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Company).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Phone).MaximumLength(20);
    }
}
