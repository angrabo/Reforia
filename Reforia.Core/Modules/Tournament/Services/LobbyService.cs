using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Reforia.Core.Common.Config.Contracts;
using Reforia.Core.Common.Config.Interfaces;
using Reforia.Core.Modules.Tournament.Models;

namespace Reforia.Core.Modules.Tournament.Services;

public partial class LobbyService
{
    private readonly IServiceScopeFactory _scopeFactory;

    private readonly ConcurrentDictionary<string, LobbyStateDto> _lobbies = new();

    [GeneratedRegex(@"^Slot\s+(?<slot>\d+)\s+(?<ready>Ready|Not Ready|No Map)\s+https?:\/\/osu\.ppy\.sh\/u\/\d+\s+(?<user>[^\s\[]+)(?:\s+\[Team\s+(?<team>Blue|Red)\s*\])?", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex PlayerRegex();

    [GeneratedRegex(@"Room name:\s*(?<name>.+?),\s*History:", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex RoomNameRegex();

    [GeneratedRegex(@"Team mode:\s*(?<mode>\w+),\s*Win condition:\s*(?<win>\w+)", RegexOptions.Compiled)]
    private static partial Regex TeamWinRegex();

    [GeneratedRegex(@"b(?:eatmaps)?\/(?<id>\d+)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex BeatmapRegex();

    [GeneratedRegex(@"^(?<user>.+?)\s+joined\s+in\s+slot\s+(?<slot>\d+)(?:\s+for\s+team\s+(?<team>blue|red))?\.", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex JoinedRegex();

    [GeneratedRegex(@"^(?<user>.+?)\s+left\s+the\s+game\.", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex LeftRegex();

    [GeneratedRegex(@"^(?<user>.+?)\s+moved\s+to\s+slot\s+(?<slot>\d+)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex MovedRegex();

    [GeneratedRegex(@"^(?<user>.+?)\s+changed\s+to\s+(?<team>Blue|Red)$", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex TeamChangeRegex();

    public LobbyService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task<LobbyStateDto?> ProcessMessage(string chatId, string message)
    {
        var text = message.Trim();
    
        BeatmapDto? fetchedBeatmap = null;
        var bmMatch = BeatmapRegex().Match(text);
        if (bmMatch.Success)
        {
            fetchedBeatmap = await FetchBeatmapData(bmMatch.Groups["id"].Value);
        }

        LobbyStateDto? finalResult = null;

        _lobbies.AddOrUpdate(chatId, 
                             _ => {
                                 var lobby = CreateDefaultLobby(chatId, null, null);
                                 var updated = InternalUpdate(lobby, text, fetchedBeatmap);
                                 finalResult = updated;
                                 return updated;
                             },
                             (_, existing) => {
                                 var updated = InternalUpdate(existing, text, fetchedBeatmap);
                                 finalResult = updated;
                                 return updated;
                             }
        );

        if (finalResult?.Status == "closed")
            _lobbies.TryRemove(chatId, out _);

        return finalResult;
    }

    private LobbyStateDto InternalUpdate(LobbyStateDto lobby, string text, BeatmapDto? map)
    {
        var updated = lobby;

        if (map != null) 
            updated = updated with { Beatmap = map };

        updated = ProcessGameStatus(updated, text);
        updated = ProcessSettings(updated, text);
        updated = ProcessPlayerActions(updated, text);

        return updated;
    }

    private LobbyStateDto ProcessGameStatus(LobbyStateDto lobby, string text)
    {
        if (lobby.Players.Count < 1)
            return lobby;

        return text switch
        {
            "All players are ready" => lobby with
            {
                Players = lobby.Players.Select(p => p with { IsReady = true }).ToList()
            },
            "The match has started!" => lobby with
            {
                Status = "progress",
                Players = lobby.Players.Select(p => p with { IsReady = false, IsPlaying = true }).ToList()
            },
            "Aborted the match" => lobby with
            {
                Status = "open",
                Players = lobby.Players.Select(p => p with { IsReady = false, IsPlaying = false }).ToList()
            },
            "The match has finished!" => lobby with
            {
                Status = "open", Players = lobby.Players.Select(p => p with { IsPlaying = false }).ToList()
            },
            "Closed the match" => lobby with { Status = "closed", Players = [] },
            _ => lobby
        };
    }

    private LobbyStateDto ProcessSettings(LobbyStateDto lobby, string text)
    {
        if (RoomNameRegex().Match(text) is { Success: true } roomMatch)
            return lobby with { DisplayName = ParseBanchoRoomName(roomMatch.Groups["name"].Value.Trim()) };

        if (text.Contains("Team mode:") && TeamWinRegex().Match(text) is { Success: true } twMatch)
            return ApplySettings(lobby, twMatch.Groups["mode"].Value, twMatch.Groups["win"].Value);

        if (text.StartsWith("Changed match settings to"))
        {
            var content = text.Replace("Changed match settings to ", "");
            var newSettings = ParseChangedSettings(lobby.Settings, content);
            return ApplySettings(lobby, newSettings.TeamMode, newSettings.WinCondition, newSettings);
        }

        if (text.StartsWith("Active mods:") || text.Contains("Enabled") || text.Contains("Disabled"))
        {
            var updatedSettings = HandleModUpdate(lobby.Settings, text);
            return updatedSettings != lobby.Settings ? lobby with { Settings = updatedSettings } : lobby;
        }

        return lobby;
    }

    private LobbyStateDto ProcessPlayerActions(LobbyStateDto lobby, string text)
    {
        if (PlayerRegex().Match(text) is { Success: true } p)
            return UpsertPlayer(
                lobby,
                new PlayerDto(int.Parse(p.Groups["slot"].Value), p.Groups["user"].Value, false,
                              p.Groups["ready"].Value == "Ready",
                              NormalizeTeam(p.Groups["team"].Value, lobby.Settings.TeamMode)));

        if (JoinedRegex().Match(text) is { Success: true } j)
            return UpsertPlayer(
                lobby,
                new PlayerDto(int.Parse(j.Groups["slot"].Value), j.Groups["user"].Value, false, false,
                              NormalizeTeam(j.Groups["team"].Value, lobby.Settings.TeamMode)));

        if (LeftRegex().Match(text) is { Success: true } l)
        {
            var username = l.Groups["user"].Value;
            return lobby with { Players = lobby.Players.Where(playerDto => playerDto.Username != username).ToList() };
        }

        if (MovedRegex().Match(text) is { Success: true } m)
        {
            var user = m.Groups["user"].Value;
            var newSlot = int.Parse(m.Groups["slot"].Value);
            return lobby with
            {
                Players = lobby.Players.Select(playerDto =>
                                                   playerDto.Username == user
                                                       ? playerDto with { Slot = newSlot }
                                                       : playerDto)
                    .OrderBy(playerDto => playerDto.Slot).ToList()
            };
        }

        if (TeamChangeRegex().Match(text) is { Success: true } t)
        {
            var user = t.Groups["user"].Value;
            var team = NormalizeTeam(t.Groups["team"].Value, lobby.Settings.TeamMode);
            return lobby with
            {
                Players = lobby.Players.Select(p => p.Username == user ? p with { Team = team } : p).ToList()
            };
        }

        return lobby;
    }

    private LobbyStateDto ApplySettings(LobbyStateDto lobby, string mode, string winCond,
                                        LobbySettingsDto? fullSettings = null)
    {
        var settings = (fullSettings ?? lobby.Settings) with { TeamMode = mode, WinCondition = winCond };
        var players = lobby.Players;
        if (IsNonTeamMode(mode))
        {
            players = players.Select(p => p with { Team = "None" }).ToList();
        }

        return lobby with { Settings = settings, Players = players };
    }

    private string NormalizeTeam(string rawTeam, string teamMode)
    {
        if (IsNonTeamMode(teamMode) || string.IsNullOrWhiteSpace(rawTeam))
            return "None";

        return char.ToUpper(rawTeam[0]) + rawTeam.Substring(1).ToLower();
    }

    private bool IsNonTeamMode(string mode) =>
        mode.Equals("HeadToHead", StringComparison.OrdinalIgnoreCase) ||
        mode.Equals("TagCoop", StringComparison.OrdinalIgnoreCase);

    private LobbySettingsDto ParseChangedSettings(LobbySettingsDto current, string content)
    {
        var parts = content.Split(", ").Select(p => p.Trim());
        var s = current;
        foreach (var part in parts)
        {
            if (part.Contains("slots"))
            {
                if (int.TryParse(part.Split(' ')[0], out int size)) s = s with { LobbySize = size };
            }
            else if (new[] { "HeadToHead", "TagCoop", "TeamVs", "TagTeamVs" }.Contains(
                         part, StringComparer.OrdinalIgnoreCase))
                s = s with { TeamMode = part };
            else s = s with { WinCondition = part };
        }

        return s;
    }

    private LobbySettingsDto HandleModUpdate(LobbySettingsDto s, string text)
    {
        if (text.StartsWith("Active mods:", StringComparison.OrdinalIgnoreCase))
        {
            return ParseFullModsList(s, text[12..].Trim());
        }

        var currentMods = new List<string>();
    
        if (text.Contains("Enabled", StringComparison.OrdinalIgnoreCase))
        {
            var enabledPart = text;
            if (text.Contains(", disabled", StringComparison.OrdinalIgnoreCase)) 
                enabledPart = text.Split(", disabled")[0];
        
            enabledPart = enabledPart.Replace("Enabled ", "", StringComparison.OrdinalIgnoreCase);
        
            var modsFound = enabledPart.Split(", ")
                .Select(m => m.Trim())
                .Where(m => !string.IsNullOrEmpty(m));

            foreach (var m in modsFound) 
                AddModToList(currentMods, m);

            if (text.Contains("FreeMod", StringComparison.OrdinalIgnoreCase)) 
                s = s with { Freemod = text.Contains("Enabled FreeMod", StringComparison.OrdinalIgnoreCase) ? "true" : "false" };

            return s with { Mods = currentMods.Count == 0 ? "None" : string.Join(", ", currentMods) };
        }

        if (text.Contains("Disabled all mods", StringComparison.OrdinalIgnoreCase))
            return s with { Mods = "None" };

        return s;
    }
        
    private void AddModToList(List<string> list, string mod)
    {
        if (string.IsNullOrWhiteSpace(mod)) return;
        
        if (!list.Any(m => m.Equals(mod, StringComparison.OrdinalIgnoreCase))) 
            list.Add(mod);
    }

    private LobbySettingsDto ParseFullModsList(LobbySettingsDto s, string modsPart)
    {
        var isFreemod = modsPart.Contains("Freemod", StringComparison.OrdinalIgnoreCase);
        var modsList = modsPart.Split(", ")
            .Select(m => m.Trim())
            .Where(m => !string.IsNullOrEmpty(m) && 
                         !m.Equals("None", StringComparison.OrdinalIgnoreCase) && 
                         !m.Equals("Freemod", StringComparison.OrdinalIgnoreCase))
            .ToList();

        return s with
        {
            Freemod = isFreemod ? "true" : "false",
            Mods = modsList.Count == 0 ? "None" : string.Join(", ", modsList)
        };
    }

    private LobbyStateDto UpsertPlayer(LobbyStateDto lobby, PlayerDto player)
    {
        var updated = lobby.Players.Where(p => p.Username != player.Username && p.Slot != player.Slot).ToList();
        updated.Add(player);
        return lobby with { Players = updated.OrderBy(p => p.Slot).ToList() };
    }

    private string ParseBanchoRoomName(string rawName)
    {
        var match = Regex.Match(rawName, @"^(?<id>.+?):\s*\((?<t1>.+?)\)\s*vs\s*\((?<t2>.+?)\)",
                                RegexOptions.IgnoreCase);
        return match.Success
                   ? $"{match.Groups["id"].Value}: {match.Groups["t1"].Value} vs. {match.Groups["t2"].Value}"
                   : rawName;
    }

    private async Task<BeatmapDto?> FetchBeatmapData(string beatmapId)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var config = scope.ServiceProvider.GetRequiredService<IConfigService>();
            var httpClient = scope.ServiceProvider.GetRequiredService<HttpClient>();

            var apiKey = await config.Get(EConfigOptions.ApiToken);

            var url = $"https://osu.ppy.sh/api/get_beatmaps?b={beatmapId}&k={apiKey}";

            var response = await httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return null;

            var body = await response.Content.ReadAsStringAsync();
            var results = JsonSerializer.Deserialize<List<OsuApiBeatmapDto>>(body);
            var apiMap = results?.FirstOrDefault();
            return apiMap != null ? BeatmapDto.FromApi(apiMap) : null;
        }
        catch
        {
            return null;
        }
    }

    private LobbyStateDto CreateDefaultLobby(string chatId, string? tournament, string? stage)
    {
        return new LobbyStateDto
        {
            Id = chatId,
            DisplayName = chatId,
            Type = chatId.StartsWith("#mp_") ? "tournament" : (chatId.StartsWith("#") ? "channel" : "user"),
            TournamentName = tournament ?? "Custom",
            Stage = stage ?? "General",
            Settings = new LobbySettingsDto(),
            Players = new List<PlayerDto>()
        };
    }
}