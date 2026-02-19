using System.Diagnostics.CodeAnalysis;

namespace ThePredictions.Contracts.Authentication;

[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
public class RequestPasswordResetRequest
{
    public string Email { get; init; } = string.Empty;
}
