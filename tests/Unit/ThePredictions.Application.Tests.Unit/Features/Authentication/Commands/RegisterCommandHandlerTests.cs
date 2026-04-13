using FluentAssertions;
using NSubstitute;
using ThePredictions.Application.Common.Exceptions;
using ThePredictions.Application.Common.Models;
using ThePredictions.Application.Features.Authentication.Commands.Register;
using ThePredictions.Application.Services;
using ThePredictions.Contracts.Authentication;
using ThePredictions.Domain.Common.Enumerations;
using ThePredictions.Domain.Models;
using Xunit;

namespace ThePredictions.Application.Tests.Unit.Features.Authentication.Commands;

public class RegisterCommandHandlerTests
{
    private readonly IUserManager _userManager = Substitute.For<IUserManager>();
    private readonly IAuthenticationTokenService _tokenService = Substitute.For<IAuthenticationTokenService>();
    private readonly RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
    {
        _handler = new RegisterCommandHandler(_userManager, _tokenService);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccessfulResponse_WhenRegistrationIsValid()
    {
        // Arrange
        var command = new RegisterCommand("John", "Doe", "john@example.com", "Password123!");
        var expiresAtUtc = new DateTime(2026, 5, 1, 12, 0, 0, DateTimeKind.Utc);

        _userManager.FindByEmailAsync(command.Email).Returns((ApplicationUser?)null);
        _userManager.CreateAsync(Arg.Any<ApplicationUser>(), command.Password)
            .Returns(UserManagerResult.Success());
        _userManager.AddToRoleAsync(Arg.Any<ApplicationUser>(), nameof(ApplicationUserRole.Player))
            .Returns(UserManagerResult.Success());
        _tokenService.GenerateTokensAsync(Arg.Any<ApplicationUser>(), Arg.Any<CancellationToken>())
            .Returns(("access-token", "refresh-token", expiresAtUtc));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<SuccessfulAuthenticationResponse>();
        var success = (SuccessfulAuthenticationResponse)result;
        success.AccessToken.Should().Be("access-token");
        success.RefreshTokenForCookie.Should().Be("refresh-token");
        success.ExpiresAtUtc.Should().Be(expiresAtUtc);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailedResponse_WhenEmailAlreadyExists()
    {
        // Arrange
        var command = new RegisterCommand("John", "Doe", "existing@example.com", "Password123!");
        var existingUser = new ApplicationUser { Email = command.Email };

        _userManager.FindByEmailAsync(command.Email).Returns(existingUser);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeOfType<FailedAuthenticationResponse>();
        var failure = (FailedAuthenticationResponse)result;
        failure.Message.Should().Contain("Registration could not be completed");
    }

    [Fact]
    public async Task Handle_ShouldNotCreateUser_WhenEmailAlreadyExists()
    {
        // Arrange
        var command = new RegisterCommand("John", "Doe", "existing@example.com", "Password123!");
        var existingUser = new ApplicationUser { Email = command.Email };

        _userManager.FindByEmailAsync(command.Email).Returns(existingUser);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _userManager.DidNotReceive().CreateAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>());
    }

    [Fact]
    public async Task Handle_ShouldThrowIdentityUpdateException_WhenCreateFails()
    {
        // Arrange
        var command = new RegisterCommand("John", "Doe", "john@example.com", "weak");

        _userManager.FindByEmailAsync(command.Email).Returns((ApplicationUser?)null);
        _userManager.CreateAsync(Arg.Any<ApplicationUser>(), command.Password)
            .Returns(UserManagerResult.Failure(new[] { "Password too weak" }));

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<IdentityUpdateException>();
    }

    [Fact]
    public async Task Handle_ShouldAssignPlayerRole_WhenRegistrationSucceeds()
    {
        // Arrange
        var command = new RegisterCommand("John", "Doe", "john@example.com", "Password123!");
        var expiresAtUtc = new DateTime(2026, 5, 1, 12, 0, 0, DateTimeKind.Utc);

        _userManager.FindByEmailAsync(command.Email).Returns((ApplicationUser?)null);
        _userManager.CreateAsync(Arg.Any<ApplicationUser>(), command.Password)
            .Returns(UserManagerResult.Success());
        _userManager.AddToRoleAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>())
            .Returns(UserManagerResult.Success());
        _tokenService.GenerateTokensAsync(Arg.Any<ApplicationUser>(), Arg.Any<CancellationToken>())
            .Returns(("access-token", "refresh-token", expiresAtUtc));

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _userManager.Received(1).AddToRoleAsync(
            Arg.Any<ApplicationUser>(),
            nameof(ApplicationUserRole.Player));
    }

    [Fact]
    public async Task Handle_ShouldGenerateTokens_WhenRegistrationSucceeds()
    {
        // Arrange
        var command = new RegisterCommand("John", "Doe", "john@example.com", "Password123!");
        var expiresAtUtc = new DateTime(2026, 5, 1, 12, 0, 0, DateTimeKind.Utc);

        _userManager.FindByEmailAsync(command.Email).Returns((ApplicationUser?)null);
        _userManager.CreateAsync(Arg.Any<ApplicationUser>(), command.Password)
            .Returns(UserManagerResult.Success());
        _userManager.AddToRoleAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>())
            .Returns(UserManagerResult.Success());
        _tokenService.GenerateTokensAsync(Arg.Any<ApplicationUser>(), Arg.Any<CancellationToken>())
            .Returns(("access-token", "refresh-token", expiresAtUtc));

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _tokenService.Received(1).GenerateTokensAsync(
            Arg.Any<ApplicationUser>(),
            Arg.Any<CancellationToken>());
    }
}
