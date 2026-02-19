using MediatR;

namespace ThePredictions.Application.Features.Authentication.Commands.RequestPasswordReset;

public record RequestPasswordResetCommand(string Email, string ResetUrlBase) : IRequest<Unit>;
