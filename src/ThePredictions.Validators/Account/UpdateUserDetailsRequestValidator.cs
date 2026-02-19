using FluentValidation;
using ThePredictions.Contracts.Account;
using ThePredictions.Validators.Common;
using System.Diagnostics.CodeAnalysis;

namespace ThePredictions.Validators.Account;

[SuppressMessage("ReSharper", "UnusedType.Global")]
public class UpdateUserDetailsRequestValidator : AbstractValidator<UpdateUserDetailsRequest>
{
    public UpdateUserDetailsRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Please enter your first name")
            .Length(2, 50).WithMessage("Your first name must be between 2 and 50 characters")
            .MustBeASafeName();

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Please enter your last name")
            .Length(2, 50).WithMessage("Your last name must be between 2 and 50 characters")
            .MustBeASafeName();

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^07\d{9}$").WithMessage("Please enter a valid 11-digit UK mobile number starting with 07.")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));
    }
}