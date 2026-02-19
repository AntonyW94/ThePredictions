using MediatR;

namespace ThePredictions.Application.Features.Admin.Rounds.Commands;

public record UpdateScoresForNextRoundCommand(int SeasonId) : IRequest;