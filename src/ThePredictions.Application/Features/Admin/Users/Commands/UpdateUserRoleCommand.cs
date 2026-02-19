using MediatR;

namespace ThePredictions.Application.Features.Admin.Users.Commands;

public record UpdateUserRoleCommand(string UserId, string NewRole) : IRequest;