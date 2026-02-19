using MediatR;
using ThePredictions.Contracts.Leagues;

namespace ThePredictions.Application.Features.Leagues.Queries;

public record GetLeagueDashboardQuery(int LeagueId,
    string UserId,
    bool IsAdmin) : IRequest<LeagueDashboardDto?>;