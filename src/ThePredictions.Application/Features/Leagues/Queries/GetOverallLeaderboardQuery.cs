using MediatR;
using ThePredictions.Contracts.Leaderboards;

namespace ThePredictions.Application.Features.Leagues.Queries;

public record GetOverallLeaderboardQuery(int LeagueId, string CurrentUserId) : IRequest<IEnumerable<LeaderboardEntryDto>>;