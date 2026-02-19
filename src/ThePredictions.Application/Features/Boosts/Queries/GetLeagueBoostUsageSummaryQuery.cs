using MediatR;
using ThePredictions.Contracts.Boosts;

namespace ThePredictions.Application.Features.Boosts.Queries;

public record GetLeagueBoostUsageSummaryQuery(
    int LeagueId,
    string CurrentUserId) : IRequest<List<BoostUsageSummaryDto>>;
