using MediatR;
using Microsoft.Extensions.Options;
using ThePredictions.Application.Configuration;
using ThePredictions.Application.Data;
using ThePredictions.Application.Services;

namespace ThePredictions.Application.Features.Leagues.Commands;

public class NotifyLeagueAdminOfJoinRequestCommandHandler(IApplicationReadDbConnection dbConnection, IEmailService emailService, IOptions<BrevoSettings> brevoSettings) : IRequestHandler<NotifyLeagueAdminOfJoinRequestCommand>
{
    private readonly BrevoSettings _brevoSettings = brevoSettings.Value;

    public async Task Handle(NotifyLeagueAdminOfJoinRequestCommand request, CancellationToken cancellationToken)
    {
        if (_brevoSettings.Templates == null)
            return;
        
        const string sql = @"
                SELECT 
                    u.[Email],
                    u.[FirstName], 
                    l.[Name] AS LeagueName,
                    s.[Name] AS SeasonName
                FROM 
                    [AspNetUsers] u 
                JOIN 
                    [Leagues] l ON u.[Id] = l.[AdministratorUserId]
                JOIN 
                    [Seasons] s ON l.[SeasonId] = s.[Id]
                WHERE 
                    l.[Id] = @LeagueId;";

        var admin = await dbConnection.QuerySingleOrDefaultAsync<LeagueAdminDto>(sql, cancellationToken, new { request.LeagueId });
        if (admin != null)
        {
            var templateId = _brevoSettings.Templates.JoinLeagueRequest;

            var parameters = new
            {
                FIRST_NAME = request.NewMemberFirstName,
                LAST_NAME = request.NewMemberLastName,
                LEAGUE_NAME = admin.LeagueName,
                SEASON_NAME = admin.SeasonName,
                ADMIN_NAME = admin.FirstName
            };

            await emailService.SendTemplatedEmailAsync(admin.Email, templateId, parameters);
        }
    }
}

public record LeagueAdminDto(string Email, string FirstName, string LeagueName, string SeasonName);