using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace ThePredictions.Domain.Common.Enumerations;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
public enum RoundStatus
{
    [Description("Draft")]
    Draft,

    [Description("Published")]
    Published,

    [Description("In Progress")]
    InProgress,

    [Description("Completed")]
    Completed
}