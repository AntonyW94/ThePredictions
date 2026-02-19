using Ardalis.GuardClauses;
using MediatR;
using ThePredictions.Application.Repositories;
using ThePredictions.Domain.Common.Enumerations;
using ThePredictions.Domain.Common.Guards;

namespace ThePredictions.Application.Features.Leagues.Commands;

public class RemoveRejectedLeagueCommandHandler : IRequestHandler<RemoveRejectedLeagueCommand>
{
    private readonly ILeagueRepository _leagueRepository;

    public RemoveRejectedLeagueCommandHandler(ILeagueRepository leagueRepository)
    {
        _leagueRepository = leagueRepository;
    }

    public async Task Handle(RemoveRejectedLeagueCommand request, CancellationToken cancellationToken)
    {
        var league = await _leagueRepository.GetByIdAsync(request.LeagueId, cancellationToken);
        Guard.Against.EntityNotFound(request.LeagueId, league, "League");

        var member = league.Members.FirstOrDefault(m => m.UserId == request.CurrentUserId);
        if (member is null || member.Status != LeagueMemberStatus.Rejected)
            throw new InvalidOperationException("You can only remove leagues with a 'Rejected' status.");
        
        league.RemoveMember(request.CurrentUserId);

        await _leagueRepository.UpdateAsync(league, cancellationToken);
    }
}