using ThePredictions.Application.Features.Admin.Rounds.Commands;
using ThePredictions.Domain.Common.Enumerations;

namespace ThePredictions.Application.Features.Admin.Rounds.Strategies;

public interface IPrizeStrategy
{
    PrizeType PrizeType { get; }
    Task AwardPrizes(ProcessPrizesCommand command, CancellationToken cancellationToken);
}