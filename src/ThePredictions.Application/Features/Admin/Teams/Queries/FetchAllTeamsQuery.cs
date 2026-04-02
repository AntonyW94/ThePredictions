using MediatR;
using ThePredictions.Contracts.Admin.Teams;

namespace ThePredictions.Application.Features.Admin.Teams.Queries;

public record FetchAllTeamsQuery(int? SeasonId = null) : IRequest<IEnumerable<TeamDto>>;