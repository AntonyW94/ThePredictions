using MediatR;
using ThePredictions.Application.Repositories;

namespace ThePredictions.Application.Features.Authentication.Commands.Logout;

public class LogoutCommandHandler(IRefreshTokenRepository refreshTokenRepository)
    : IRequestHandler<LogoutCommand>
{
    public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        await refreshTokenRepository.RevokeAllForUserAsync(request.UserId, cancellationToken);
    }
}

