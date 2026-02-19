using MediatR;
using ThePredictions.Contracts.Admin.Teams;

namespace ThePredictions.Application.Features.Admin.Teams.Commands;

public record CreateTeamCommand (
    string Name,
    string ShortName,
    string LogoUrl,
    string Abbreviation,
    int? ApiTeamId) : IRequest<TeamDto>;