using MediatR;
using ThePredictions.Contracts.Leagues;

namespace ThePredictions.Application.Features.Dashboard.Queries;

public record GetAvailableLeaguesQuery(string UserId) : IRequest<IEnumerable<AvailableLeagueDto>>;