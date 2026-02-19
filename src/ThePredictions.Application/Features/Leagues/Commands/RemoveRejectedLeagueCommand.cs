using MediatR;
using ThePredictions.Application.Common.Interfaces;

namespace ThePredictions.Application.Features.Leagues.Commands;

public record RemoveRejectedLeagueCommand(
    int LeagueId,
    string CurrentUserId) : IRequest, ITransactionalRequest;