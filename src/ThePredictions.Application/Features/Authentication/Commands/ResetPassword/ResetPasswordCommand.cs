using MediatR;
using ThePredictions.Contracts.Authentication;

namespace ThePredictions.Application.Features.Authentication.Commands.ResetPassword;

public record ResetPasswordCommand(
    string Token,
    string NewPassword
) : IRequest<ResetPasswordResponse>;
