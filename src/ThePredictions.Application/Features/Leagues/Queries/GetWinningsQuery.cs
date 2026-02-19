using MediatR;
using ThePredictions.Contracts.Leagues;

namespace ThePredictions.Application.Features.Leagues.Queries;

public record GetWinningsQuery(int LeagueId, string CurrentUserId) : IRequest<WinningsDto>;