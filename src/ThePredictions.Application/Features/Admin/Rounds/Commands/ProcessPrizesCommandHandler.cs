using MediatR;
using ThePredictions.Application.Features.Admin.Rounds.Strategies;
using ThePredictions.Application.Repositories;

namespace ThePredictions.Application.Features.Admin.Rounds.Commands;

public class ProcessPrizesCommandHandler(IEnumerable<IPrizeStrategy> prizeStrategies, ILeagueRepository leagueRepository) : IRequestHandler<ProcessPrizesCommand, Unit>
{
    public async Task<Unit> Handle(ProcessPrizesCommand request, CancellationToken cancellationToken)
    {
        var league = await leagueRepository.GetByIdWithAllDataAsync(request.LeagueId, cancellationToken);
        if (league == null || !league.PrizeSettings.Any())
            return Unit.Value;
        
        foreach (var prizeSetting in league.PrizeSettings)
        {
            var strategy = prizeStrategies.FirstOrDefault(s => s.PrizeType == prizeSetting.PrizeType);
            if (strategy != null)
                await strategy.AwardPrizes(request, cancellationToken);
        }
        
        return Unit.Value;
    }
}