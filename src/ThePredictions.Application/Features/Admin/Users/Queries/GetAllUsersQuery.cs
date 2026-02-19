using MediatR;
using ThePredictions.Contracts.Admin.Users;

namespace ThePredictions.Application.Features.Admin.Users.Queries;

public record GetAllUsersQuery : IRequest<IEnumerable<UserDto>>;