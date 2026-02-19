using MediatR;
using ThePredictions.Contracts.Leaderboards;

namespace ThePredictions.Application.Features.Leagues.Queries;

public record GetMonthlyLeaderboardQuery(int LeagueId, int Month, string CurrentUserId) : IRequest<IEnumerable<LeaderboardEntryDto>>;