using MediatR;

namespace ThePredictions.Application.Features.Admin.Users.Queries;

public record UserOwnsLeaguesQuery(string UserId) : IRequest<bool>;