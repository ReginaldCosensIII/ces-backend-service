using CES.BackendService.Contracts;
using FluentValidation;

namespace CES.BackendService.Validation;

public class ForumRegistrationRequestValidator : AbstractValidator<ForumRegistrationRequest>
{
    public ForumRegistrationRequestValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().WithMessage("First Name is required.").MaximumLength(100).WithMessage("First Name cannot exceed 100 characters.");
        RuleFor(x => x.LastName).NotEmpty().WithMessage("Last Name is required.").MaximumLength(100).WithMessage("Last Name cannot exceed 100 characters.");
        RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required.").MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");
        RuleFor(x => x.Company).NotEmpty().WithMessage("Company is required.").MaximumLength(200).WithMessage("Company cannot exceed 200 characters.");
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email address is required.").EmailAddress().WithMessage("Please enter a valid email address.");
        RuleFor(x => x.Phone).MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters.");
    }
}
