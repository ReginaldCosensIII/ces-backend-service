using CES.BackendService.Contracts;
using FluentValidation;

namespace CES.BackendService.Validation;

public class SeminarRegistrationRequestValidator : AbstractValidator<SeminarRegistrationRequest>
{
    public SeminarRegistrationRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().WithMessage("First Name is required.");
        RuleFor(x => x.LastName).NotEmpty().WithMessage("Last Name is required.");
        RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required.");
        RuleFor(x => x.Company).NotEmpty().WithMessage("Company is required.");
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email is required.")
                             .EmailAddress().WithMessage("A valid email address is required.");
    }
}
