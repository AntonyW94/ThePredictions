using MediatR;
using ThePredictions.Contracts.Admin.Rounds;

namespace ThePredictions.Application.Features.Admin.Rounds.Queries;

public record GetRoundByIdQuery(int Id) : IRequest<RoundDetailsDto?>;