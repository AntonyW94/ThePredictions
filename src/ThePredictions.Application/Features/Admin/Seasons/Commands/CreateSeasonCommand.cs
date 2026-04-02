using MediatR;
using ThePredictions.Application.Common.Interfaces;
using ThePredictions.Contracts.Admin.Seasons;
using ThePredictions.Domain.Common.Enumerations;

namespace ThePredictions.Application.Features.Admin.Seasons.Commands;

public record CreateSeasonCommand(
    string Name,
    DateTime StartDateUtc,
    DateTime EndDateUtc,
    string CreatorId,
    bool IsActive,
    int NumberOfRounds,
    int? ApiLeagueId,
    CompetitionType CompetitionType) : IRequest<SeasonDto>, ITransactionalRequest;