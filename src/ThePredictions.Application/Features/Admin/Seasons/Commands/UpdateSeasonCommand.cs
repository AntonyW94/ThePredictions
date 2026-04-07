using MediatR;
using ThePredictions.Contracts.Admin.Seasons;
using ThePredictions.Domain.Common.Enumerations;

namespace ThePredictions.Application.Features.Admin.Seasons.Commands;

public record UpdateSeasonCommand(
    int Id,
    string Name,
    DateTime StartDateUtc,
    DateTime EndDateUtc,
    bool IsActive,
    int NumberOfRounds,
    int? ApiLeagueId,
    CompetitionType CompetitionType,
    List<TournamentRoundMappingDto> TournamentRoundMappings) : IRequest;