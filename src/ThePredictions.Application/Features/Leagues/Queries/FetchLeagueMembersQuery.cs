using MediatR;
using ThePredictions.Contracts.Leagues;

namespace ThePredictions.Application.Features.Leagues.Queries;

public record FetchLeagueMembersQuery(int LeagueId, string CurrentUserId) : IRequest<LeagueMembersPageDto?>;