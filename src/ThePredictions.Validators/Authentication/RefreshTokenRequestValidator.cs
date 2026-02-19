using FluentValidation;
using ThePredictions.Contracts.Authentication;
using System.Diagnostics.CodeAnalysis;

namespace ThePredictions.Validators.Authentication;

[SuppressMessage("ReSharper", "UnusedType.Global")]
public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty()
            .When(x => x.Token is not null)
            .WithMessage("Refresh token cannot be empty when provided.")
            .MaximumLength(500)
            .WithMessage("Refresh token is too long.");
    }
}
