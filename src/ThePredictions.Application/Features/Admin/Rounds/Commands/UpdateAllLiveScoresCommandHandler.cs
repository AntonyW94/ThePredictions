using MediatR;
using ThePredictions.Application.Repositories;

namespace ThePredictions.Application.Features.Admin.Rounds.Commands;

public class UpdateAllLiveScoresCommandHandler(ISeasonRepository seasonRepository, IMediator mediator) : IRequestHandler<UpdateAllLiveScoresCommand>
{
    public async Task Handle(UpdateAllLiveScoresCommand request, CancellationToken cancellationToken)
    {
        var activeSeasons = await seasonRepository.GetActiveSeasonsAsync(cancellationToken);

        foreach (var season in activeSeasons)
        {
            await mediator.Send(new UpdateScoresForNextRoundCommand (season.Id) , cancellationToken);
        }
    }
}