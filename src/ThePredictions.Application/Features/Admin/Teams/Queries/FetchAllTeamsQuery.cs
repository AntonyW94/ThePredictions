using MediatR;
using ThePredictions.Contracts.Admin.Teams;

namespace ThePredictions.Application.Features.Admin.Teams.Queries;

public record FetchAllTeamsQuery : IRequest<IEnumerable<TeamDto>>;