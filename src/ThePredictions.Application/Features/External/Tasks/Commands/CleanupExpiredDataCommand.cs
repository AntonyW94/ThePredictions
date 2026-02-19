using System.Diagnostics.CodeAnalysis;
using MediatR;

namespace ThePredictions.Application.Features.External.Tasks.Commands;

public record CleanupExpiredDataCommand : IRequest<CleanupResult>;

[SuppressMessage("ReSharper", "NotAccessedPositionalProperty.Global")]
public record CleanupResult(int PasswordResetTokensDeleted);
