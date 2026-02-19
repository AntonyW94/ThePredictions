using MediatR;
using ThePredictions.Contracts.Admin.Seasons;

namespace ThePredictions.Application.Features.Admin.Seasons.Queries;

public record GetSeasonByIdQuery(int Id) : IRequest<SeasonDto?>;