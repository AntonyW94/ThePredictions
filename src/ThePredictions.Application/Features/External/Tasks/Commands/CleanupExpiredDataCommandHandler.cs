using MediatR;
using Microsoft.Extensions.Logging;
using ThePredictions.Application.Repositories;

namespace ThePredictions.Application.Features.External.Tasks.Commands;

public class CleanupExpiredDataCommandHandler(
    IPasswordResetTokenRepository passwordResetTokenRepository,
    ILogger<CleanupExpiredDataCommandHandler> logger) : IRequestHandler<CleanupExpiredDataCommand, CleanupResult>
{
    private const int PasswordResetTokenRetentionDays = 30;

    public async Task<CleanupResult> Handle(CleanupExpiredDataCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting scheduled cleanup task");

        // Clean up password reset tokens older than 30 days
        var tokenCutoffDate = DateTime.UtcNow.AddDays(-PasswordResetTokenRetentionDays);
        var tokensDeleted = await passwordResetTokenRepository.DeleteTokensOlderThanAsync(
            tokenCutoffDate,
            cancellationToken);

        if (tokensDeleted > 0)
        {
            logger.LogInformation(
                "Deleted {TokensDeleted} password reset tokens older than {CutoffDate:yyyy-MM-dd}",
                tokensDeleted,
                tokenCutoffDate);
        }

        logger.LogInformation("Scheduled cleanup task completed");

        return new CleanupResult(
            PasswordResetTokensDeleted: tokensDeleted
        );
    }
}
