using System.Text.Json.Serialization;

namespace ThePredictions.Application.FootballApi.DTOs;

public class RoundsResponseWrapper
{
    [JsonPropertyName("response")]
    public string[] Response { get; init; } = null!;
}