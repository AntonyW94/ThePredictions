using MediatR;
using ThePredictions.Application.Common.Interfaces;
using ThePredictions.Contracts.Leagues;

namespace ThePredictions.Application.Features.Leagues.Commands;

public record DefinePrizeStructureCommand(
    int LeagueId,
    string DefiningUserId,
    List<DefinePrizeSettingDto> PrizeSettings
) : IRequest, ITransactionalRequest;