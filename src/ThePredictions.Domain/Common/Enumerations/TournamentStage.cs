using System.Diagnostics.CodeAnalysis;

namespace ThePredictions.Domain.Common.Enumerations;

/// <summary>
/// Used during tournament sync to classify API round names and determine grouping logic.
/// Not persisted to the database.
/// </summary>
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public enum TournamentStage
{
    Group,
    RoundOf32,
    RoundOf16,
    QuarterFinals,
    SemiFinals,
    ThirdPlace,
    Final
}
