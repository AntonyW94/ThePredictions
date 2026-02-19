using MediatR;
using ThePredictions.Contracts.Leagues;

namespace ThePredictions.Application.Features.Leagues.Queries;

public record GetCreateLeaguePageDataQuery : IRequest<CreateLeaguePageData>;