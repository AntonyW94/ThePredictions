using MediatR;

namespace PredictionLeague.Application.Features.External.Tasks.Commands;

public record CleanupExpiredDataCommand : IRequest<CleanupResult>;

public record CleanupResult(int PasswordResetTokensDeleted);
