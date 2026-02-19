using MediatR;
using ThePredictions.Contracts.Leagues;

namespace ThePredictions.Application.Features.Leagues.Queries;

public record GetManageLeaguesQuery(
    string UserId,
    bool IsAdmin) : IRequest<ManageLeaguesDto>;