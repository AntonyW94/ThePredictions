using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace ThePredictions.Application.FootballApi.DTOs;

[ExcludeFromCodeCoverage]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class Score
{
    [JsonPropertyName("halftime")]
    public ScoreDetail? Halftime { get; set; }

    [JsonPropertyName("fulltime")]
    public ScoreDetail? Fulltime { get; set; }

    [JsonPropertyName("extratime")]
    public ScoreDetail? Extratime { get; set; }

    [JsonPropertyName("penalty")]
    public ScoreDetail? Penalty { get; set; }
}
