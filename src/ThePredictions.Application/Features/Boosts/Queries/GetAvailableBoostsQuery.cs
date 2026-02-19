using MediatR;
using ThePredictions.Contracts.Boosts;

namespace ThePredictions.Application.Features.Boosts.Queries;

public record GetAvailableBoostsQuery(int LeagueId, int RoundId, string UserId) : IRequest<List<BoostOptionDto>>;