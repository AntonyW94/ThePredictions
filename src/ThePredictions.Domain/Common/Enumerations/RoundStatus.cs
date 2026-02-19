using System.Diagnostics.CodeAnalysis;

namespace ThePredictions.Domain.Common.Enumerations;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
public enum RoundStatus
{
    Draft,      
    Published, 
    InProgress,
    Completed 
}