using Reforia.Core.Common.Dto;

namespace Reforia.Core.Modules.Communication.Functions.Response;

public class GetBeatmapInfoFunctionResponse
{
    public string Id { get; set; }
    public string SetId { get; set; }
    public string Artist { get; set; }
    public string Title { get; set; }
    public string Version { get; set; }
    public string Creator { get; set; }
    public string CircleSize { get; set; }
    public string ApproachRate { get; set; }
    public string OverallDifficulty { get; set; }
    public string HealthDrain { get; set; }
    public string Bpm { get; set; }
    public string Length { get; set; }
    public string StarRating { get; set; }
    
    
    public GetBeatmapInfoFunctionResponse(BeatmapDto beatmap)
    {
        ArgumentNullException.ThrowIfNull(beatmap);

        Id = beatmap.BeatmapId;
        SetId = beatmap.BeatmapSetId;
        Artist = beatmap.Artist;
        Title = beatmap.Title;
        Version = beatmap.Version;
        Creator = beatmap.Creator;
        CircleSize = beatmap.CircleSize;
        ApproachRate = beatmap.ApproachRate;
        OverallDifficulty = beatmap.OverallDifficulty;
        HealthDrain = beatmap.HealthDrain;
        Bpm = beatmap.Bpm;
        Length = beatmap.TotalLength;
        StarRating = beatmap.StarRating;
    }
}