using ThePredictions.Domain.Common.Enumerations;

namespace ThePredictions.Contracts.Dashboard;

/// <summary>
/// Lightweight DTO for displaying match predictions on the dashboard active rounds tile.
/// Contains only the data needed for the compact match preview (logos, scores, and outcome).
/// </summary>
public record ActiveRoundMatchDto(
    string? HomeTeamLogoUrl,
    string? AwayTeamLogoUrl,
    int? PredictedHomeScore,
    int? PredictedAwayScore,
    PredictionOutcome? Outcome,
    MatchStatus Status
);
