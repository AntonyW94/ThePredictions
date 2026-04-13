using System.Security.Authentication;
using FluentAssertions;
using NSubstitute;
using ThePredictions.Application.Features.Leagues.Commands;
using ThePredictions.Application.Repositories;
using ThePredictions.Domain.Common.Exceptions;
using ThePredictions.Domain.Models;
using Xunit;

namespace ThePredictions.Application.Tests.Unit.Features.Leagues.Commands;

public class DeleteLeagueCommandHandlerTests
{
    private readonly ILeagueRepository _leagueRepository = Substitute.For<ILeagueRepository>();
    private readonly DeleteLeagueCommandHandler _handler;

    public DeleteLeagueCommandHandlerTests()
    {
        _handler = new DeleteLeagueCommandHandler(_leagueRepository);
    }

    private static League CreateLeague(int id = 1, string administratorUserId = "admin-user")
    {
        return new League(
            id: id, name: "Test League", seasonId: 1,
            administratorUserId: administratorUserId,
            entryCode: "ABC123",
            createdAtUtc: new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            entryDeadlineUtc: new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc),
            pointsForExactScore: 3, pointsForCorrectResult: 1,
            price: 0, isFree: true, hasPrizes: false,
            prizeFundOverride: null,
            members: null, prizeSettings: null);
    }

    [Fact]
    public async Task Handle_ShouldDeleteLeague_WhenUserIsLeagueAdministrator()
    {
        // Arrange
        var league = CreateLeague(id: 1, administratorUserId: "admin-user");
        var command = new DeleteLeagueCommand(1, "admin-user", false);

        _leagueRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(league);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _leagueRepository.Received(1).DeleteAsync(1, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldDeleteLeague_WhenUserIsSiteAdmin()
    {
        // Arrange
        var league = CreateLeague(id: 1, administratorUserId: "other-user");
        var command = new DeleteLeagueCommand(1, "site-admin", true);

        _leagueRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(league);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _leagueRepository.Received(1).DeleteAsync(1, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldThrowAuthenticationException_WhenUserIsNotAuthorised()
    {
        // Arrange
        var league = CreateLeague(id: 1, administratorUserId: "admin-user");
        var command = new DeleteLeagueCommand(1, "other-user", false);

        _leagueRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns(league);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<AuthenticationException>()
            .WithMessage("*not authorised*");
    }

    [Fact]
    public async Task Handle_ShouldThrowEntityNotFoundException_WhenLeagueNotFound()
    {
        // Arrange
        var command = new DeleteLeagueCommand(999, "user-1", false);

        _leagueRepository.GetByIdAsync(999, Arg.Any<CancellationToken>())
            .Returns((League?)null);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }
}
