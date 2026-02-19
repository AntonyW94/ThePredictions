using MediatR;

namespace ThePredictions.Application.Features.Admin.Seasons.Commands;

public record RecalculateSeasonStatsCommand(int SeasonId) : IRequest;