using Ardalis.GuardClauses;
using MediatR;
using ThePredictions.Application.Repositories;
using ThePredictions.Domain.Common;
using ThePredictions.Domain.Common.Enumerations;
using ThePredictions.Domain.Common.Guards;

namespace ThePredictions.Application.Features.Leagues.Commands;

public class UpdateLeagueMemberStatusCommandHandler : IRequestHandler<UpdateLeagueMemberStatusCommand>
{
    private readonly ILeagueRepository _leagueRepository;
    private readonly ILeagueMemberRepository _leagueMemberRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UpdateLeagueMemberStatusCommandHandler(ILeagueRepository leagueRepository, ILeagueMemberRepository leagueMemberRepository, IDateTimeProvider dateTimeProvider)
    {
        _leagueRepository = leagueRepository;
        _leagueMemberRepository = leagueMemberRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task Handle(UpdateLeagueMemberStatusCommand request, CancellationToken cancellationToken)
    {
        var league = await _leagueRepository.GetByIdAsync(request.LeagueId, cancellationToken);
        Guard.Against.EntityNotFound(request.LeagueId, league, "League");

        if (league.AdministratorUserId != request.UpdatingUserId)
            throw new UnauthorizedAccessException("Only the league administrator can update member status.");
        
        var member = await _leagueMemberRepository.GetAsync(request.LeagueId, request.MemberId, cancellationToken);
        Guard.Against.EntityNotFound(request.MemberId, member, "LeagueMember");

        switch (request.NewStatus)
        {
            case LeagueMemberStatus.Approved:
                member.Approve(_dateTimeProvider);
                break;

            case LeagueMemberStatus.Rejected:
                member.Reject();
                break;

            case LeagueMemberStatus.Pending:
                break;
            
            default:
                throw new InvalidOperationException("This status change is not permitted.");
        }

        await _leagueMemberRepository.UpdateAsync(member, cancellationToken);
    }
}