using MediatR;

namespace ThePredictions.Application.Features.Admin.Rounds.Commands;

public class ProcessPrizesCommand : IRequest<Unit>
{
    public int RoundId { get; init; }
    public int LeagueId { get; init; }
}