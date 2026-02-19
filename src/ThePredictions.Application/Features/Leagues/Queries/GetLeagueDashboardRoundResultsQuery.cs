using MediatR;
using ThePredictions.Contracts.Leagues;

namespace ThePredictions.Application.Features.Leagues.Queries;

public record GetLeagueDashboardRoundResultsQuery(
    int LeagueId,
    int RoundId,
    string CurrentUserId) : IRequest<IEnumerable<PredictionResultDto>?>;