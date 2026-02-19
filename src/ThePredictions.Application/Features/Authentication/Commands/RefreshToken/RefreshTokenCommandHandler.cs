using MediatR;
using Microsoft.Extensions.Logging;
using ThePredictions.Application.Repositories;
using ThePredictions.Application.Services;
using ThePredictions.Contracts.Authentication;
using ThePredictions.Domain.Common;

namespace ThePredictions.Application.Features.Authentication.Commands.RefreshToken;

public class RefreshTokenCommandHandler(
    IUserManager userManager,
    IRefreshTokenRepository refreshTokenRepository,
    IAuthenticationTokenService tokenService,
    IDateTimeProvider dateTimeProvider,
    ILogger<RefreshTokenCommandHandler> logger)
    : IRequestHandler<RefreshTokenCommand, AuthenticationResponse>
{
    public async Task<AuthenticationResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("RefreshTokenCommandHandler started.");

        if (string.IsNullOrEmpty(request.RefreshToken))
        {
            logger.LogWarning("Handler received a null or empty refresh token.");
            return new FailedAuthenticationResponse("Refresh token not found.");
        }

        var correctedToken = request.RefreshToken.Replace(' ', '+');
        logger.LogDebug("Token format corrected (space replacement applied)");

        var storedToken = await refreshTokenRepository.GetByTokenAsync(correctedToken, cancellationToken);
        if (storedToken == null || !storedToken.IsActive(dateTimeProvider))
        {
            logger.LogWarning("Refresh token validation failed - token not found or inactive");
            return new FailedAuthenticationResponse("Invalid or expired refresh token.");
        }
        logger.LogInformation("Successfully found active token in the database for user ID: {UserId}", storedToken.UserId);

        var user = await userManager.FindByIdAsync(storedToken.UserId);
        if (user == null)
        {
            logger.LogError("User not found for UserId: {UserId} associated with the refresh token.", storedToken.UserId);
            return new FailedAuthenticationResponse("User not found.");
        }
        logger.LogInformation("Successfully found User (ID: {UserId})", user.Id);

        storedToken.Revoke(dateTimeProvider);
        await refreshTokenRepository.UpdateAsync(storedToken, cancellationToken);

        var (accessToken, newRefreshToken, expiresAt) = await tokenService.GenerateTokensAsync(user, cancellationToken);
        logger.LogInformation("Successfully generated new tokens for User (ID: {UserId})", user.Id);

        return new SuccessfulAuthenticationResponse(accessToken, expiresAt, newRefreshToken);
    }
}
