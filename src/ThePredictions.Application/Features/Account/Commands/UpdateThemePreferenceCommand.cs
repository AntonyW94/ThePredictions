using MediatR;

namespace ThePredictions.Application.Features.Account.Commands;

public record UpdateThemePreferenceCommand(
    string UserId,
    string Theme) : IRequest;
