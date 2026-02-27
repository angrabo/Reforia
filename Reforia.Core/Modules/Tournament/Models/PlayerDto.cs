namespace Reforia.Core.Modules.Tournament.Models;

public record PlayerDto(
    int Slot,
    string Username,
    bool IsPlaying,
    bool IsReady,
    string Team
);
