using MediatR;
using ThePredictions.Contracts.Homepage;

namespace ThePredictions.Application.Features.Homepage.Queries;

public record GetHomepageSeasonsQuery : IRequest<IEnumerable<HomepageSeasonDto>>;
