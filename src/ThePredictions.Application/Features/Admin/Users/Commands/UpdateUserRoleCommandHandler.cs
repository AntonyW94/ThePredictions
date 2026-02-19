using Ardalis.GuardClauses;
using MediatR;
using ThePredictions.Application.Services;
using ThePredictions.Domain.Common.Guards;

namespace ThePredictions.Application.Features.Admin.Users.Commands;

public class UpdateUserRoleCommandHandler(IUserManager userManager, ICurrentUserService currentUserService)
    : IRequestHandler<UpdateUserRoleCommand>
{
    public async Task Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
    {
        currentUserService.EnsureAdministrator();

        var user = await userManager.FindByIdAsync(request.UserId);
        Guard.Against.EntityNotFound(request.UserId, user, "User");

        var currentRoles = await userManager.GetRolesAsync(user);
        await userManager.RemoveFromRolesAsync(user, currentRoles);

        var result = await userManager.AddToRoleAsync(user, request.NewRole);
        if (!result.Succeeded)
            throw new Exception($"Failed to update role: {string.Join(", ", result.Errors)}");
    }
}