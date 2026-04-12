using MediatR;
using ThePredictions.Contracts.Dashboard;

namespace ThePredictions.Application.Features.Dashboard.Queries;

public record GetPendingMembersForAdminQuery(string UserId) : IRequest<PendingMembersResultDto>;
