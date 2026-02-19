using MediatR;

namespace ThePredictions.Application.Features.Admin.Seasons.Commands;

public record SyncSeasonWithApiCommand(int SeasonId) : IRequest;