using Ardalis.GuardClauses;
using MediatR;
using ThePredictions.Application.Repositories;
using ThePredictions.Application.Services;
using ThePredictions.Domain.Common.Guards;

namespace ThePredictions.Application.Features.Admin.Teams.Commands;

public class UpdateTeamCommandHandler(ITeamRepository teamRepository, ICurrentUserService currentUserService)
    : IRequestHandler<UpdateTeamCommand>
{
    public async Task Handle(UpdateTeamCommand request, CancellationToken cancellationToken)
    {
        currentUserService.EnsureAdministrator();

        var team = await teamRepository.GetByIdAsync(request.Id, cancellationToken);
        Guard.Against.EntityNotFound(request.Id, team, "Team");

        team.UpdateDetails(request.Name, request.ShortName, request.LogoUrl, request.Abbreviation, request.ApiTeamId);

        await teamRepository.UpdateAsync(team, cancellationToken);
    }
}