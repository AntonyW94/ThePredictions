using MediatR;
using ThePredictions.Application.Repositories;
using ThePredictions.Application.Services;
using ThePredictions.Contracts.Admin.Rounds;
using ThePredictions.Domain.Models;

namespace ThePredictions.Application.Features.Admin.Rounds.Commands;

public class CreateRoundCommandHandler(IRoundRepository roundRepository, ICurrentUserService currentUserService) : IRequestHandler<CreateRoundCommand, RoundDto>
{
    public async Task<RoundDto> Handle(CreateRoundCommand request, CancellationToken cancellationToken)
    {
        currentUserService.EnsureAdministrator();

        var round = Round.Create(
            request.SeasonId,
            request.RoundNumber,
            request.StartDateUtc,
            request.DeadlineUtc,
            request.ApiRoundName);

        foreach (var matchToAdd in request.Matches)
        {
            round.AddMatch(matchToAdd.HomeTeamId, matchToAdd.AwayTeamId, matchToAdd.MatchDateTimeUtc, matchToAdd.ExternalId);
        }

        var createdRound = await roundRepository.CreateAsync(round, cancellationToken);

        return new RoundDto
        (
            createdRound.Id,
            createdRound.SeasonId,
            createdRound.RoundNumber,
            createdRound.ApiRoundName,
            createdRound.StartDateUtc,
            createdRound.DeadlineUtc,
            createdRound.Status,
            createdRound.Matches.Count
        );
    }
}