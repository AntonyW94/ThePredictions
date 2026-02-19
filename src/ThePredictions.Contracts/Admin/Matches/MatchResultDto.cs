using ThePredictions.Domain.Common.Enumerations;

namespace ThePredictions.Contracts.Admin.Matches;

public record MatchResultDto(int MatchId, int HomeScore, int AwayScore, MatchStatus Status);