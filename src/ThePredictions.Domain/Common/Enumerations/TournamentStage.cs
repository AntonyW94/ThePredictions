using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace ThePredictions.Domain.Common.Enumerations;

/// <summary>
/// Tournament stages used to configure round structure and match API round names during sync.
/// Persisted as pipe-delimited strings in TournamentRoundMappings.Stages.
/// </summary>
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public enum TournamentStage
{
    [Description("Group Stage - Matchday 1")]
    Group1,

    [Description("Group Stage - Matchday 2")]
    Group2,

    [Description("Group Stage - Matchday 3")]
    Group3,

    [Description("Round of 32")]
    RoundOf32,

    [Description("Round of 16")]
    RoundOf16,

    [Description("Quarter-finals")]
    QuarterFinals,

    [Description("Semi-finals")]
    SemiFinals,

    [Description("Third Place Playoff")]
    ThirdPlace,

    [Description("Final")]
    Final
}
