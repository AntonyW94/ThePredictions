using MediatR;
using ThePredictions.Contracts.Dashboard;

namespace ThePredictions.Application.Features.Dashboard.Queries;

public record GetPendingRequestsQuery(string UserId) : IRequest<IEnumerable<LeagueRequestDto>>;