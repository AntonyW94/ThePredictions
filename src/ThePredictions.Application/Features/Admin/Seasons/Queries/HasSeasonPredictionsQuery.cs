using MediatR;

namespace ThePredictions.Application.Features.Admin.Seasons.Queries;

public record HasSeasonPredictionsQuery(int SeasonId) : IRequest<bool>;
