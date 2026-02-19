using MediatR;
using ThePredictions.Application.Common.Interfaces;

namespace ThePredictions.Application.Features.Leagues.Commands;

public record DeleteLeagueCommand(
    int LeagueId,
    string DeletingUserId,
    bool IsAdmin) : IRequest, ITransactionalRequest;