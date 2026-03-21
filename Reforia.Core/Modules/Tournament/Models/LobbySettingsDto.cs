namespace Reforia.Core.Modules.Tournament.Models;

public record LobbySettingsDto(
    bool Freemod = false,
    Mods Mods = Mods.None,
    string WinCondition = "Score",
    string TeamMode = "HeadToHead",
    int LobbySize = 16
);