using MediatR;
using ThePredictions.Application.Repositories;

namespace ThePredictions.Application.Features.Admin.Rounds.Commands;

public class UpdateAllLiveScoresCommandHandler : IRequestHandler<UpdateAllLiveScoresCommand>
{
    private readonly ISeasonRepository _seasonRepository;
    private readonly IMediator _mediator;

    public UpdateAllLiveScoresCommandHandler(ISeasonRepository seasonRepository, IMediator mediator)
    {
        _seasonRepository = seasonRepository;
        _mediator = mediator;
    }

    public async Task Handle(UpdateAllLiveScoresCommand request, CancellationToken cancellationToken)
    {
        var activeSeasons = await _seasonRepository.GetActiveSeasonsAsync(cancellationToken);

        foreach (var season in activeSeasons)
        {
            await _mediator.Send(new UpdateScoresForNextRoundCommand (season.Id) , cancellationToken);
        }
    }
}