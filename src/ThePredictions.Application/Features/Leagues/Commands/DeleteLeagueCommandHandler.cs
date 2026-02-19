using Ardalis.GuardClauses;
using MediatR;
using ThePredictions.Application.Repositories;
using ThePredictions.Domain.Common.Guards;
using System.Security.Authentication;

namespace ThePredictions.Application.Features.Leagues.Commands;

public class DeleteLeagueCommandHandler : IRequestHandler<DeleteLeagueCommand>
{
    private readonly ILeagueRepository _leagueRepository;

    public DeleteLeagueCommandHandler(ILeagueRepository leagueRepository)
    {
        _leagueRepository = leagueRepository;
    }

    public async Task Handle(DeleteLeagueCommand request, CancellationToken cancellationToken)
    {
        var league = await _leagueRepository.GetByIdAsync(request.LeagueId, cancellationToken);
        Guard.Against.EntityNotFound(request.LeagueId, league, "League");
       
        if (league.AdministratorUserId != request.DeletingUserId && !request.IsAdmin)
            throw new AuthenticationException("You are not authorized to delete this league.");
        
        await _leagueRepository.DeleteAsync(request.LeagueId, cancellationToken);
    }
}