using MediatR;
using ThePredictions.Contracts.Leagues;

namespace ThePredictions.Application.Features.Leagues.Queries;

public record GetLeagueByIdQuery(int Id, string CurrentUserId) : IRequest<LeagueDto?>;