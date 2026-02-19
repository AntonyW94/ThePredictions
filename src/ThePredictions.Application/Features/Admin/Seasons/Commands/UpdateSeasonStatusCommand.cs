using MediatR;

namespace ThePredictions.Application.Features.Admin.Seasons.Commands;

public record UpdateSeasonStatusCommand(int SeasonId, bool IsActive) : IRequest;