using MediatR;
using ThePredictions.Contracts.Admin.Rounds;

namespace ThePredictions.Application.Features.Admin.Rounds.Queries;

public record FetchRoundsForSeasonQuery(int SeasonId) : IRequest<IEnumerable<RoundDto>>;