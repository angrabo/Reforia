namespace Reforia.Core.Modules.Tournament.Models;

public record BeatmapDto(
    string BeatmapId,
    string BeatmapsetId,
    string TotalLength,
    string Title,
    string Artist,
    string Version,
    string Creator,
    string MaxCombo,
    string CircleSize,
    string OverallDifficulty,
    string ApproachRate,
    string HealthDrain,
    string Bpm,
    string StarRating
)
{
    public static BeatmapDto FromApi(OsuApiBeatmapDto api) => new(
        BeatmapId: api.BeatmapId,
        BeatmapsetId: api.BeatmapsetId,
        TotalLength: api.TotalLength,
        Title: api.Title,
        Artist: api.Artist,
        Version: api.Version,
        Creator: api.Creator,
        MaxCombo: api.MaxCombo,
        CircleSize: api.CircleSize,
        OverallDifficulty: api.OverallDifficulty,
        ApproachRate: api.ApproachRate,
        HealthDrain: api.HealthDrain,
        Bpm: api.Bpm,
        StarRating: api.StarRating
    );
}