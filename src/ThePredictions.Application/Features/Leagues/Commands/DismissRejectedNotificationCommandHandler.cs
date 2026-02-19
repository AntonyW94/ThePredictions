using Ardalis.GuardClauses;
using MediatR;
using ThePredictions.Application.Repositories;
using ThePredictions.Domain.Common.Enumerations;
using ThePredictions.Domain.Common.Guards;

namespace ThePredictions.Application.Features.Leagues.Commands;

public class DismissRejectedNotificationCommandHandler : IRequestHandler<DismissRejectedNotificationCommand>
{
    private readonly ILeagueMemberRepository _leagueMemberRepository;

    public DismissRejectedNotificationCommandHandler(ILeagueMemberRepository leagueMemberRepository)
    {
        _leagueMemberRepository = leagueMemberRepository;
    }

    public async Task Handle(DismissRejectedNotificationCommand request, CancellationToken cancellationToken)
    {
        var member = await _leagueMemberRepository.GetAsync(request.LeagueId, request.UserId, cancellationToken);
        Guard.Against.EntityNotFound(request.UserId, member, "League Notification");

        if (member.Status != LeagueMemberStatus.Rejected)
            throw new InvalidOperationException("This notification cannot be dismissed.");

        member.DismissAlert();

        await _leagueMemberRepository.UpdateAsync(member, cancellationToken);
    }
}