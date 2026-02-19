using MediatR;
using ThePredictions.Contracts.Leagues;

namespace ThePredictions.Application.Features.Leagues.Queries;

public record FetchAllLeaguesQuery : IRequest<IEnumerable<LeagueDto>>;