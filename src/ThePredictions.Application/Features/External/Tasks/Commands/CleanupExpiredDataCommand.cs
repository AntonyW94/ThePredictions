using MediatR;

namespace ThePredictions.Application.Features.External.Tasks.Commands;

public record CleanupExpiredDataCommand : IRequest<CleanupResult>;

public record CleanupResult(int PasswordResetTokensDeleted);
