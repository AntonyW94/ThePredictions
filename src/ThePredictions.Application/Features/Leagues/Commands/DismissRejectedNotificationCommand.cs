using MediatR;

namespace ThePredictions.Application.Features.Leagues.Commands;

public record DismissRejectedNotificationCommand(int LeagueId, string UserId) : IRequest;