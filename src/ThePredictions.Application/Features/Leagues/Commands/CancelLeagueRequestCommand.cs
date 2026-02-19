using MediatR;

namespace ThePredictions.Application.Features.Leagues.Commands;

public record CancelLeagueRequestCommand(int LeagueId, string UserId) : IRequest;