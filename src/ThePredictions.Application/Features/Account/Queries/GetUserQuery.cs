using MediatR;
using ThePredictions.Contracts.Account;

namespace ThePredictions.Application.Features.Account.Queries;

public record GetUserQuery(string UserId) : IRequest<UserDetails?>;