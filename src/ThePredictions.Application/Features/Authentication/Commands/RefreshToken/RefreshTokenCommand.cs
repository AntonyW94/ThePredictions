using MediatR;
using ThePredictions.Contracts.Authentication;

namespace ThePredictions.Application.Features.Authentication.Commands.RefreshToken;

public record RefreshTokenCommand(string RefreshToken) : IRequest<AuthenticationResponse>;
