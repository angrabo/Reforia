namespace Reforia.Core.Modules.Tournament.Models;

public record LobbySettingsDto(
    string Freemod = "false",
    string Mods = "None",
    string WinCondition = "Score",
    string TeamMode = "HeadToHead",
    int LobbySize = 16
);