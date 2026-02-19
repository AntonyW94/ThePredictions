using System.Text.Json.Serialization;

namespace ThePredictions.Application.FootballApi.DTOs;

public class FixtureResponseWrapper
{
    [JsonPropertyName("response")]
    public FixtureResponse[] Response { get; init; } = null!;
}