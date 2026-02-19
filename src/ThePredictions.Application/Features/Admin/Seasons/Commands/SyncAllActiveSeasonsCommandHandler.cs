using MediatR;
using ThePredictions.Application.Repositories;

namespace ThePredictions.Application.Features.Admin.Seasons.Commands;

public class SyncAllActiveSeasonsCommandHandler(ISeasonRepository seasonRepository, IMediator mediator) : IRequestHandler<SyncAllActiveSeasonsCommand>
{
    public async Task Handle(SyncAllActiveSeasonsCommand request, CancellationToken cancellationToken)
    {
        var activeSeasons = await seasonRepository.GetActiveSeasonsAsync(cancellationToken);

        foreach (var season in activeSeasons)
        {
            await mediator.Send(new SyncSeasonWithApiCommand(season.Id), cancellationToken);
        }
    }
}