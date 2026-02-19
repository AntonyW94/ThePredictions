using MediatR;
using ThePredictions.Contracts.Authentication;

namespace ThePredictions.Application.Features.Authentication.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<AuthenticationResponse>;