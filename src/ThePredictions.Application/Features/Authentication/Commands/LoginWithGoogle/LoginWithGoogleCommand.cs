using MediatR;
using Microsoft.AspNetCore.Authentication;
using ThePredictions.Contracts.Authentication;

namespace ThePredictions.Application.Features.Authentication.Commands.LoginWithGoogle;

public record LoginWithGoogleCommand(
    AuthenticateResult AuthenticateResult,
    string Source
) : IRequest<AuthenticationResponse>;