using MediatR;
using ThePredictions.Contracts.Leagues;

namespace ThePredictions.Application.Features.Leagues.Queries;

public record GetMonthsForLeagueQuery(int LeagueId, string CurrentUserId) : IRequest<IEnumerable<MonthDto>>;