using FluentValidation;
using ThePredictions.Contracts.Admin.Users;
using ThePredictions.Domain.Common.Enumerations;
using System.Diagnostics.CodeAnalysis;

namespace ThePredictions.Validators.Admin.Users;

[SuppressMessage("ReSharper", "UnusedType.Global")]
public class UpdateUserRoleRequestValidator : AbstractValidator<UpdateUserRoleRequest>
{
    public UpdateUserRoleRequestValidator()
    {
        RuleFor(x => x.NewRole)
            .NotEmpty()
            .WithMessage("Role is required.")
            .Must(role => Enum.TryParse<ApplicationUserRole>(role, ignoreCase: true, out _))
            .WithMessage($"Role must be one of: {string.Join(", ", Enum.GetNames<ApplicationUserRole>())}");
    }
}
