using MediatR;
using ThePredictions.Application.Common.Interfaces;
using ThePredictions.Contracts.Admin.Matches;

namespace ThePredictions.Application.Features.Admin.Rounds.Commands;

public record UpdateMatchResultsCommand(
    int RoundId,
    List<MatchResultDto> Matches) : IRequest, ITransactionalRequest;