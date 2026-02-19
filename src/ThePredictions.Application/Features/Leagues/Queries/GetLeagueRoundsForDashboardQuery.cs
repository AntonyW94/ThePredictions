using MediatR;
using ThePredictions.Contracts.Admin.Rounds;

namespace ThePredictions.Application.Features.Leagues.Queries;

public record GetLeagueRoundsForDashboardQuery(int LeagueId, string CurrentUserId) : IRequest<IEnumerable<RoundDto>>;