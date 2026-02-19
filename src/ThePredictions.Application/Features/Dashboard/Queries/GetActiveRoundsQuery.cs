using MediatR;
using ThePredictions.Contracts.Dashboard;

namespace ThePredictions.Application.Features.Dashboard.Queries;

public record GetActiveRoundsQuery(string UserId) : IRequest<IEnumerable<ActiveRoundDto>>;
