using Ardalis.GuardClauses;
using MediatR;
using ThePredictions.Application.Repositories;
using ThePredictions.Domain.Common.Enumerations;
using ThePredictions.Domain.Common.Guards;

namespace ThePredictions.Application.Features.Leagues.Commands;

public class CancelLeagueRequestCommandHandler(ILeagueMemberRepository leagueMemberRepository) : IRequestHandler<CancelLeagueRequestCommand>
{
    public async Task Handle(CancelLeagueRequestCommand request, CancellationToken cancellationToken)
    {
        var member = await leagueMemberRepository.GetAsync(request.LeagueId, request.UserId, cancellationToken);
        Guard.Against.EntityNotFound(request.UserId, member, "League Join Request");
     
        if (member.Status != LeagueMemberStatus.Pending)
            throw new InvalidOperationException("You can only cancel requests that are currently pending.");

        await leagueMemberRepository.DeleteAsync(member, cancellationToken);
    }
}