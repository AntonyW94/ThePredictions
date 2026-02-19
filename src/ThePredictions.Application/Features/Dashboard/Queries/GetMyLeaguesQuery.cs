using MediatR;
using ThePredictions.Contracts.Leagues;

namespace ThePredictions.Application.Features.Dashboard.Queries;

public record GetMyLeaguesQuery(string UserId) : IRequest<IEnumerable<MyLeagueDto>>;