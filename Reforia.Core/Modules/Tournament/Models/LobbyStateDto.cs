namespace Reforia.Core.Modules.Tournament.Models;

public record LobbyStateDto
{
    public string Id { get; init; } = string.Empty;
    public string Type { get; init; } = "channel"; // "channel" | "user" | "tournament"
    public string DisplayName { get; init; } = string.Empty;
    public string? TournamentName { get; init; }
    public string? Stage { get; init; }
    public string Status { get; init; } = string.Empty; // "open", "closed", "playing"
    public LobbySettingsDto Settings { get; init; } = new();
    public List<PlayerDto> Players { get; init; } = new();
    public BeatmapDto? Beatmap { get; init; }
}