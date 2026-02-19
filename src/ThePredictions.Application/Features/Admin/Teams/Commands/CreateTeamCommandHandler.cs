using MediatR;
using ThePredictions.Application.Repositories;
using ThePredictions.Application.Services;
using ThePredictions.Contracts.Admin.Teams;
using ThePredictions.Domain.Models;

namespace ThePredictions.Application.Features.Admin.Teams.Commands;

public class CreateTeamCommandHandler(ITeamRepository teamRepository, ICurrentUserService currentUserService)
    : IRequestHandler<CreateTeamCommand, TeamDto>
{
    public async Task<TeamDto> Handle(CreateTeamCommand request, CancellationToken cancellationToken)
    {
        currentUserService.EnsureAdministrator();

        var team = Team.Create(request.Name, request.ShortName, request.LogoUrl, request.Abbreviation, request.ApiTeamId);

        var createdTeam = await teamRepository.CreateAsync(team, cancellationToken);
        return new TeamDto(createdTeam.Id, createdTeam.Name, request.ShortName, createdTeam.LogoUrl, request.Abbreviation, request.ApiTeamId);
    }
}