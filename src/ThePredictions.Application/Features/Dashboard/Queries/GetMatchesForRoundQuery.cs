using MediatR;
using ThePredictions.Contracts.Admin.Rounds;

namespace ThePredictions.Application.Features.Dashboard.Queries;

public record GetMatchesForRoundQuery(int RoundId) : IRequest<IEnumerable<MatchInRoundDto>>;