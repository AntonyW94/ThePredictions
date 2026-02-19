using MediatR;
using ThePredictions.Application.Common.Interfaces;

namespace ThePredictions.Application.Features.Admin.Users.Commands;

public record DeleteUserCommand(
    string UserIdToDelete,
    string DeletingUserId,
    string? NewAdministratorId
) : IRequest, ITransactionalRequest;