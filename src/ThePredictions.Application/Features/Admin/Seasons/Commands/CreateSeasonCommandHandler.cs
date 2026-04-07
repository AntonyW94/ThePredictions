using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using ThePredictions.Application.Features.Admin.Teams.Queries;
using ThePredictions.Application.Repositories;
using ThePredictions.Application.Services;
using ThePredictions.Contracts.Admin.Seasons;
using ThePredictions.Domain.Common;
using ThePredictions.Domain.Common.Enumerations;
using ThePredictions.Domain.Models;

namespace ThePredictions.Application.Features.Admin.Seasons.Commands;

public class CreateSeasonCommandHandler(
    ISeasonRepository seasonRepository,
    ILeagueRepository leagueRepository,
    IRoundRepository roundRepository,
    ITournamentRoundMappingRepository tournamentRoundMappingRepository,
    IFootballDataService footballDataService,
    IMediator mediator,
    ICurrentUserService currentUserService,
    IDateTimeProvider dateTimeProvider,
    ILogger<CreateSeasonCommandHandler> logger) : IRequestHandler<CreateSeasonCommand, SeasonDto>
{
    public async Task<SeasonDto> Handle(CreateSeasonCommand request, CancellationToken cancellationToken)
    {
        currentUserService.EnsureAdministrator();

        await ValidateSeasonAgainstApiAsync(request, cancellationToken);

        var season = CreateSeasonEntity(request);
        var createdSeason = await seasonRepository.CreateAsync(season, cancellationToken);

        if (createdSeason.IsTournament && request.TournamentRoundMappings.Any())
        {
            await SaveTournamentMappingsAndCreatePlaceholderRoundsAsync(createdSeason, request.TournamentRoundMappings, cancellationToken);
        }

        if (createdSeason.ApiLeagueId.HasValue)
            await mediator.Send(new SyncSeasonWithApiCommand(createdSeason.Id), cancellationToken);

        var publicLeague = CreatePublicLeagueEntity(request, createdSeason);
        await leagueRepository.CreateAsync(publicLeague, cancellationToken);

        return MapToSeasonDto(createdSeason);
    }

    private async Task ValidateSeasonAgainstApiAsync(CreateSeasonCommand request, CancellationToken cancellationToken)
    {
        if (!request.ApiLeagueId.HasValue)
            return;

        var seasonYear = request.StartDateUtc.Year;
        var validationFailures = new List<ValidationFailure>();

        try
        {
            var apiSeason = await footballDataService.GetLeagueSeasonDetailsAsync(request.ApiLeagueId.Value, seasonYear, cancellationToken);
            if (apiSeason == null)
                throw new ValidationException($"The API returned no season data for League ID {request.ApiLeagueId.Value} and Year {seasonYear}. Please verify the details.");
            
            if (request.StartDateUtc.Date != apiSeason.Start.Date)
                validationFailures.Add(new ValidationFailure(nameof(request.StartDateUtc), $"The Start Date does not match the API. Expected: {apiSeason.Start:yyyy-MM-dd}, but you entered: {request.StartDateUtc:yyyy-MM-dd}."));

            if (request.EndDateUtc.Date != apiSeason.End.Date)
                validationFailures.Add(new ValidationFailure(nameof(request.EndDateUtc), $"The End Date does not match the API. Expected: {apiSeason.End:yyyy-MM-dd}, but you entered: {request.EndDateUtc:yyyy-MM-dd}."));
            
            var apiRoundNames = (await footballDataService.GetRoundsForSeasonAsync(request.ApiLeagueId.Value, seasonYear, cancellationToken)).ToList();
            if (request.NumberOfRounds != apiRoundNames.Count)
                validationFailures.Add(new ValidationFailure(nameof(request.NumberOfRounds), $"The Number of Rounds does not match the API. Expected: {apiRoundNames.Count}, but you entered: {request.NumberOfRounds}."));
            
            var apiTeams = (await footballDataService.GetTeamsForSeasonAsync(request.ApiLeagueId.Value, seasonYear, cancellationToken)).ToList();
            var localTeams = await mediator.Send(new FetchAllTeamsQuery(), cancellationToken);

            var localTeamApiIds = localTeams
                .Where(t => t.ApiTeamId.HasValue)
                .Select(t => t.ApiTeamId.GetValueOrDefault())
                .ToHashSet();

            var missingTeams = apiTeams
                .Where(apiTeam => !localTeamApiIds.Contains(apiTeam.Team.Id))
                .Select(apiTeam => apiTeam.Team.Name)
                .ToList();

            if (missingTeams.Any())
                validationFailures.Add(new ValidationFailure(nameof(request.ApiLeagueId), $"The following teams from the API do not exist in the database: {string.Join(", ", missingTeams)}. Please add them before creating the season."));
        }
        catch (HttpRequestException ex)
        {
            throw new ValidationException($"Could not retrieve data from the football API. Please check your API key. Details: {ex.Message}");
        }

        if (validationFailures.Any())
            throw new ValidationException(validationFailures);
    }

    private async Task SaveTournamentMappingsAndCreatePlaceholderRoundsAsync(
        Season season,
        List<TournamentRoundMappingDto> mappingDtos,
        CancellationToken cancellationToken)
    {
        var mappings = mappingDtos.Select(dto =>
            TournamentRoundMapping.Create(
                season.Id,
                dto.RoundNumber,
                dto.DisplayName,
                string.Join("|", dto.Stages),
                dto.ExpectedMatchCount)).ToList();

        await tournamentRoundMappingRepository.ReplaceAllForSeasonAsync(season.Id, mappings, cancellationToken);

        foreach (var mapping in mappings)
        {
            var stages = mapping.GetStageList();
            var round = Round.Create(
                season.Id,
                mapping.RoundNumber,
                mapping.DisplayName,
                season.StartDateUtc,
                season.StartDateUtc.AddMinutes(-30),
                apiRoundName: null);

            var createdRound = await roundRepository.CreateAsync(round, cancellationToken);

            var matchNumber = 1;
            for (var i = 0; i < mapping.ExpectedMatchCount; i++)
            {
                // For combined rounds, assign matches to stages proportionally
                var stage = stages.Count == 1
                    ? stages[0]
                    : GetStageForMatchIndex(stages, i, mapping.ExpectedMatchCount);

                var placeholderName = TournamentRoundNameParser.GetPlaceholderMatchName(stage, matchNumber);
                var apiRoundNameForStage = TournamentRoundNameParser.GetDefaultDisplayName(stage);

                createdRound.AddPlaceholderMatch(placeholderName, placeholderName, apiRoundNameForStage);
                matchNumber++;
            }

            await roundRepository.UpdateAsync(createdRound, cancellationToken);
            logger.LogInformation("Round (ID: {RoundId}) created with {MatchCount} placeholder matches for tournament Season (ID: {SeasonId})", createdRound.Id, createdRound.Matches.Count, season.Id);
        }
    }

    private static TournamentStage GetStageForMatchIndex(List<TournamentStage> stages, int matchIndex, int totalMatches)
    {
        // For combined knockout rounds, distribute matches across stages
        // using known knockout stage sizes (SF=2, ThirdPlace=1, Final=1)
        var cumulative = 0;
        foreach (var stage in stages)
        {
            var stageSize = stage switch
            {
                TournamentStage.SemiFinals => 2,
                TournamentStage.ThirdPlace => 1,
                TournamentStage.Final => 1,
                TournamentStage.QuarterFinals => 4,
                TournamentStage.RoundOf16 => 8,
                TournamentStage.RoundOf32 => 16,
                _ => totalMatches / stages.Count
            };

            cumulative += stageSize;
            if (matchIndex < cumulative)
                return stage;
        }

        return stages[^1];
    }

    private static Season CreateSeasonEntity(CreateSeasonCommand request)
    {
        return Season.Create(
            request.Name,
            request.StartDateUtc,
            request.EndDateUtc,
            request.IsActive,
            request.NumberOfRounds,
            request.ApiLeagueId,
            request.CompetitionType);
    }

    private League CreatePublicLeagueEntity(CreateSeasonCommand request, Season createdSeason)
    {
        return League.CreateOfficialPublicLeague(
            createdSeason.Id,
            createdSeason.Name,
            0,
            request.CreatorId,
            createdSeason.StartDateUtc.AddDays(-1),
            createdSeason,
            dateTimeProvider
        );
    }

    private static SeasonDto MapToSeasonDto(Season createdSeason)
    {
        return new SeasonDto(
            createdSeason.Id,
            createdSeason.Name,
            createdSeason.StartDateUtc,
            createdSeason.EndDateUtc,
            createdSeason.IsActive,
            createdSeason.NumberOfRounds,
            (int)createdSeason.CompetitionType,
            createdSeason.ApiLeagueId,
            0, 0, 0, 0, 0
        );
    }
}