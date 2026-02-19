using MediatR;
using ThePredictions.Application.Common.Interfaces;
using ThePredictions.Contracts.Admin.Matches;
using ThePredictions.Contracts.Admin.Rounds;

namespace ThePredictions.Application.Features.Admin.Rounds.Commands;

public record CreateRoundCommand(
    int SeasonId, 
    int RoundNumber,
    string ApiRoundName,
    DateTime StartDateUtc,
    DateTime DeadlineUtc, 
    List<CreateMatchRequest> Matches) : IRequest<RoundDto>, ITransactionalRequest;