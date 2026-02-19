using MediatR;

namespace ThePredictions.Application.Features.Leagues.Commands;

public record NotifyLeagueAdminOfJoinRequestCommand(int LeagueId,
    string NewMemberFirstName,
    string NewMemberLastName) : IRequest;