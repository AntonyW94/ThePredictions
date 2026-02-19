using MediatR;

namespace ThePredictions.Application.Features.Dashboard.Queries;

public record CheckForAvailablePrivateLeaguesQuery(string UserId) : IRequest<bool>;