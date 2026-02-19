using System.Text.Json.Serialization;

namespace ThePredictions.Application.FootballApi.DTOs;

public class TeamResponseWrapper
{
    [JsonPropertyName("response")]
    public TeamResponse[] Response { get; init; } = null!;
}