using PredictionLeague.Domain.Common.Enumerations;

namespace PredictionLeague.Contracts.Admin.Matches;

public record MatchResultDto(int MatchId, int HomeScore, int AwayScore, MatchStatus Status);