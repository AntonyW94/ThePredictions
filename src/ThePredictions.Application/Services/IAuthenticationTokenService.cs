using ThePredictions.Domain.Models;

namespace ThePredictions.Application.Services;

public interface IAuthenticationTokenService
{
    Task<(string AccessToken, string RefreshToken, DateTime ExpiresAtUtc)> GenerateTokensAsync(ApplicationUser user, CancellationToken cancellationToken);
}