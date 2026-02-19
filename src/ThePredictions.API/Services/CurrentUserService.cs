using System.Security.Claims;
using ThePredictions.Application.Services;
using ThePredictions.Domain.Common.Constants;

namespace ThePredictions.API.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public string? UserId => httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

    public bool IsAuthenticated => httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;

    public bool IsAdministrator => httpContextAccessor.HttpContext?.User.IsInRole(RoleNames.Administrator) ?? false;

    public void EnsureAdministrator()
    {
        if (!IsAuthenticated)
            throw new UnauthorizedAccessException("Authentication is required to access this resource.");

        if (!IsAdministrator)
            throw new UnauthorizedAccessException("Administrator privileges are required to access this resource.");
    }
}
