using FluentValidation;
using ThePredictions.Contracts.Authentication;
using System.Diagnostics.CodeAnalysis;

namespace ThePredictions.Validators.Authentication;

[SuppressMessage("ReSharper", "UnusedType.Global")]
public class RequestPasswordResetRequestValidator : AbstractValidator<RequestPasswordResetRequest>
{
    public RequestPasswordResetRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Please enter your email address.");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Please enter a valid email address.")
            .When(x => !string.IsNullOrEmpty(x.Email));
    }
}
