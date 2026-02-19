using MediatR;
using ThePredictions.Contracts.Admin.Seasons;

namespace ThePredictions.Application.Features.Admin.Seasons.Queries;

public record FetchAllSeasonsQuery : IRequest<IEnumerable<SeasonDto>>;