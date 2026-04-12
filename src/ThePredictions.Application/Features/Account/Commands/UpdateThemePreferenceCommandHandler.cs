using Ardalis.GuardClauses;
using MediatR;
using ThePredictions.Application.Common.Exceptions;
using ThePredictions.Application.Services;
using ThePredictions.Domain.Common.Guards;

namespace ThePredictions.Application.Features.Account.Commands;

public class UpdateThemePreferenceCommandHandler(IUserManager userManager) : IRequestHandler<UpdateThemePreferenceCommand>
{
    public async Task Handle(UpdateThemePreferenceCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId);
        Guard.Against.EntityNotFound(request.UserId, user, "User");

        user.PreferredTheme = request.Theme is "dark" ? "dark" : "light";

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new IdentityUpdateException(result.Errors);
    }
}
