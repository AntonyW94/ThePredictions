using FluentValidation;
using ThePredictions.Contracts.Admin.Users;
using System.Diagnostics.CodeAnalysis;

namespace ThePredictions.Validators.Admin.Users;

[SuppressMessage("ReSharper", "UnusedType.Global")]
public class DeleteUserRequestValidator : AbstractValidator<DeleteUserRequest>
{
    public DeleteUserRequestValidator()
    {
        RuleFor(x => x.NewAdministratorId)
            .NotEmpty()
            .When(x => x.NewAdministratorId is not null)
            .WithMessage("New administrator ID cannot be empty when specified.");
    }
}
