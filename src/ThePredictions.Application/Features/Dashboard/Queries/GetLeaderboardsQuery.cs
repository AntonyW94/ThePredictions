using MediatR;
using ThePredictions.Contracts.Leaderboards;

namespace ThePredictions.Application.Features.Dashboard.Queries;

public record GetLeaderboardsQuery(string UserId) : IRequest<IEnumerable<LeagueLeaderboardDto>>;