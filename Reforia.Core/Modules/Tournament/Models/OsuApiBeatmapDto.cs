using System.Text.Json.Serialization;

namespace Reforia.Core.Modules.Tournament.Models;

public record OsuApiBeatmapDto(
    [property: JsonPropertyName("beatmap_id")] string BeatmapId,
    [property: JsonPropertyName("beatmapset_id")] string BeatmapsetId,
    [property: JsonPropertyName("total_length")] string TotalLength,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("artist")] string Artist,
    [property: JsonPropertyName("version")] string Version,
    [property: JsonPropertyName("creator")] string Creator,
    [property: JsonPropertyName("max_combo")] string MaxCombo,
    [property: JsonPropertyName("diff_size")] string CircleSize,
    [property: JsonPropertyName("diff_overall")] string OverallDifficulty,
    [property: JsonPropertyName("diff_approach")] string ApproachRate,
    [property: JsonPropertyName("diff_drain")] string HealthDrain,
    [property: JsonPropertyName("bpm")] string Bpm,
    [property: JsonPropertyName("difficultyrating")] string StarRating
);