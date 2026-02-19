using MediatR;
using Microsoft.Extensions.Logging;
using ThePredictions.Application.Repositories;
using ThePredictions.Application.Services;
using ThePredictions.Contracts.Authentication;
using ThePredictions.Domain.Common;

namespace ThePredictions.Application.Features.Authentication.Commands.ResetPassword;

public class ResetPasswordCommandHandler(
    IPasswordResetTokenRepository tokenRepository,
    IUserManager userManager,
    IAuthenticationTokenService tokenService,
    IDateTimeProvider dateTimeProvider,
    ILogger<ResetPasswordCommandHandler> logger)
    : IRequestHandler<ResetPasswordCommand, ResetPasswordResponse>
{
    public async Task<ResetPasswordResponse> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        // Clean up expired tokens (opportunistic cleanup)
        await tokenRepository.DeleteExpiredTokensAsync(cancellationToken);

        // Look up the token
        var resetToken = await tokenRepository.GetByTokenAsync(request.Token, cancellationToken);

        if (resetToken == null)
        {
            logger.LogWarning("Password reset attempted with non-existent token");
            return new FailedResetPasswordResponse("The password reset link is invalid or has expired.");
        }

        if (resetToken.IsExpired(dateTimeProvider))
        {
            logger.LogWarning("Password reset attempted with expired token for User (ID: {UserId})", resetToken.UserId);
            await tokenRepository.DeleteAsync(request.Token, cancellationToken);
            return new FailedResetPasswordResponse("The password reset link is invalid or has expired.");
        }

        // Look up the user
        var user = await userManager.FindByIdAsync(resetToken.UserId);

        if (user == null)
        {
            logger.LogWarning("Password reset token references non-existent User (ID: {UserId})", resetToken.UserId);
            await tokenRepository.DeleteAsync(request.Token, cancellationToken);
            return new FailedResetPasswordResponse("The password reset link is invalid or has expired.");
        }

        // Reset the password using Identity's password hasher
        var result = await userManager.ResetPasswordDirectAsync(user, request.NewPassword);

        if (!result.Succeeded)
        {
            logger.LogWarning("Password reset failed for User (ID: {UserId}). Errors: {Errors}",
                user.Id, string.Join(", ", result.Errors));

            // Return the first error (usually password policy violation)
            var errorMessage = result.Errors.FirstOrDefault() ?? "Password reset failed.";
            return new FailedResetPasswordResponse(errorMessage);
        }

        // Delete the used token (and any other tokens for this user)
        await tokenRepository.DeleteByUserIdAsync(user.Id, cancellationToken);

        logger.LogInformation("Password successfully reset for User (ID: {UserId})", user.Id);

        // Auto-login: Generate tokens for the user
        var (accessToken, refreshToken, expiresAtUtc) = await tokenService.GenerateTokensAsync(user, cancellationToken);

        return new SuccessfulResetPasswordResponse(
            AccessToken: accessToken,
            ExpiresAtUtc: expiresAtUtc,
            RefreshTokenForCookie: refreshToken
        );
    }
}
