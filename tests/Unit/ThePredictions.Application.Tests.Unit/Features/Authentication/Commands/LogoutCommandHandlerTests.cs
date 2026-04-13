using NSubstitute;
using ThePredictions.Application.Features.Authentication.Commands.Logout;
using ThePredictions.Application.Repositories;
using Xunit;

namespace ThePredictions.Application.Tests.Unit.Features.Authentication.Commands;

public class LogoutCommandHandlerTests
{
    private readonly IRefreshTokenRepository _refreshTokenRepository = Substitute.For<IRefreshTokenRepository>();
    private readonly LogoutCommandHandler _handler;

    public LogoutCommandHandlerTests()
    {
        _handler = new LogoutCommandHandler(_refreshTokenRepository);
    }

    [Fact]
    public async Task Handle_ShouldRevokeAllTokensForUser_WhenCalled()
    {
        // Arrange
        var command = new LogoutCommand("user-1");

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _refreshTokenRepository.Received(1).RevokeAllForUserAsync("user-1", Arg.Any<CancellationToken>());
    }
}
