using MediatR;
using ThePredictions.Application.Services;
using ThePredictions.Contracts.Authentication;

namespace ThePredictions.Application.Features.Authentication.Commands.Login;

public class LoginCommandHandler(IUserManager userManager, IAuthenticationTokenService tokenService)
    : IRequestHandler<LoginCommand, AuthenticationResponse>
{
    public async Task<AuthenticationResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null || !await userManager.CheckPasswordAsync(user, request.Password))
            return new FailedAuthenticationResponse("Invalid email or password.");

        var (accessToken, refreshToken, expiresAtUtc) = await tokenService.GenerateTokensAsync(user, cancellationToken);

        return new SuccessfulAuthenticationResponse(
            AccessToken: accessToken,
            RefreshTokenForCookie: refreshToken,
            ExpiresAtUtc: expiresAtUtc
        );
    }
}