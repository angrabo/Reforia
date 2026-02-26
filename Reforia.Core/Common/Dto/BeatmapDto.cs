using System.Text.Json.Serialization;

namespace Reforia.Core.Common.Dto;

public class BeatmapDto
{
    [JsonPropertyName("beatmap_id")]
    public string BeatmapId { get; set; }
    
    [JsonPropertyName("beatmapset_id")]
    public string BeatmapSetId { get; set; }
    
    [JsonPropertyName("total_length")]
    public string TotalLength { get; set; }
    
    [JsonPropertyName("title")]
    public string Title { get; set; }
    
    [JsonPropertyName("artist")]
    public string Artist { get; set; }
    
    [JsonPropertyName("version")]
    public string Version { get; set; }
    
    [JsonPropertyName("creator")]
    public string Creator { get; set; }
    
    [JsonPropertyName("max_combo")]
    public string MaxCombo { get; set; }
    
    [JsonPropertyName("diff_size")]
    public string CircleSize { get; set;}
    
    [JsonPropertyName("diff_overall")]
    public string OverallDifficulty { get; set; }
    
    [JsonPropertyName("diff_approach")]
    public string ApproachRate { get; set; }
    
    [JsonPropertyName("diff_drain")]
    public string HealthDrain { get; set; }
    
    [JsonPropertyName("bpm")]
    public string Bpm { get; set; }
    
    [JsonPropertyName("difficultyrating")]
    public string StarRating { get; set; }
    
}