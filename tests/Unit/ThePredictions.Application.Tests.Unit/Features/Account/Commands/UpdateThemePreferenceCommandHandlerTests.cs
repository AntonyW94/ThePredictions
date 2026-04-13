using FluentAssertions;
using NSubstitute;
using ThePredictions.Application.Common.Exceptions;
using ThePredictions.Application.Common.Models;
using ThePredictions.Application.Features.Account.Commands;
using ThePredictions.Application.Services;
using ThePredictions.Domain.Common.Exceptions;
using ThePredictions.Domain.Models;
using Xunit;

namespace ThePredictions.Application.Tests.Unit.Features.Account.Commands;

public class UpdateThemePreferenceCommandHandlerTests
{
    private readonly IUserManager _userManager = Substitute.For<IUserManager>();
    private readonly UpdateThemePreferenceCommandHandler _handler;

    public UpdateThemePreferenceCommandHandlerTests()
    {
        _handler = new UpdateThemePreferenceCommandHandler(_userManager);
    }

    [Fact]
    public async Task Handle_ShouldSetThemeToDark_WhenThemeIsDark()
    {
        // Arrange
        var user = new ApplicationUser { Id = "user-1", PreferredTheme = "light" };
        var command = new UpdateThemePreferenceCommand("user-1", "dark");

        _userManager.FindByIdAsync("user-1").Returns(user);
        _userManager.UpdateAsync(user).Returns(UserManagerResult.Success());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        user.PreferredTheme.Should().Be("dark");
        await _userManager.Received(1).UpdateAsync(user);
    }

    [Fact]
    public async Task Handle_ShouldSetThemeToLight_WhenThemeIsLight()
    {
        // Arrange
        var user = new ApplicationUser { Id = "user-1", PreferredTheme = "dark" };
        var command = new UpdateThemePreferenceCommand("user-1", "light");

        _userManager.FindByIdAsync("user-1").Returns(user);
        _userManager.UpdateAsync(user).Returns(UserManagerResult.Success());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        user.PreferredTheme.Should().Be("light");
    }

    [Fact]
    public async Task Handle_ShouldDefaultToLight_WhenThemeIsUnrecognised()
    {
        // Arrange
        var user = new ApplicationUser { Id = "user-1", PreferredTheme = "dark" };
        var command = new UpdateThemePreferenceCommand("user-1", "neon-pink");

        _userManager.FindByIdAsync("user-1").Returns(user);
        _userManager.UpdateAsync(user).Returns(UserManagerResult.Success());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        user.PreferredTheme.Should().Be("light");
    }

    [Fact]
    public async Task Handle_ShouldThrowEntityNotFoundException_WhenUserNotFound()
    {
        // Arrange
        var command = new UpdateThemePreferenceCommand("unknown-user", "dark");

        _userManager.FindByIdAsync("unknown-user").Returns((ApplicationUser?)null);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task Handle_ShouldThrowIdentityUpdateException_WhenUpdateFails()
    {
        // Arrange
        var user = new ApplicationUser { Id = "user-1", PreferredTheme = "light" };
        var command = new UpdateThemePreferenceCommand("user-1", "dark");

        _userManager.FindByIdAsync("user-1").Returns(user);
        _userManager.UpdateAsync(user).Returns(UserManagerResult.Failure(new[] { "Update failed" }));

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<IdentityUpdateException>();
    }
}
