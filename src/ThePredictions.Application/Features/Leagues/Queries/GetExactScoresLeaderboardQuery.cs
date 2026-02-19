using MediatR;
using ThePredictions.Contracts.Leaderboards;

namespace ThePredictions.Application.Features.Leagues.Queries;

public record GetExactScoresLeaderboardQuery(int LeagueId, string CurrentUserId) : IRequest<ExactScoresLeaderboardDto>;