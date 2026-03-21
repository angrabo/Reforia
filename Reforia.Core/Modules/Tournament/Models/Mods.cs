namespace Reforia.Core.Modules.Tournament.Models;

[Flags]
public enum Mods
{
    None           = 0,
    NoFail         = 1,
    Easy           = 2,
    TouchDevice    = 4,
    Hidden         = 8,
    HardRock       = 16,
    SuddenDeath    = 32,
    DoubleTime     = 64,
    Relax          = 128,
    HalfTime       = 256,
    Nightcore      = 512, // Only set along with DoubleTime. i.e: NC only gives 576
    Flashlight     = 1024,
    Autoplay       = 2048,
    SpunOut        = 4096,
    Relax2         = 8192,    // Autopilot
    Perfect        = 16384, // Only set along with SuddenDeath. i.e: PF only gives 16416  
    Key4           = 32768,
    Key5           = 65536,
    Key6           = 131072,
    Key7           = 262144,
    Key8           = 524288,
    FadeIn         = 1048576,
    Random         = 2097152,
    Cinema         = 4194304,
    Target         = 8388608,
    Key9           = 16777216,
    KeyCoop        = 33554432,
    Key1           = 67108864,
    Key3           = 134217728,
    Key2           = 268435456,
    ScoreV2        = 536870912,
    Mirror         = 1073741824,
    KeyMod = Key1 | Key2 | Key3 | Key4 | Key5 | Key6 | Key7 | Key8 | Key9 | KeyCoop,
    FreeModAllowed = NoFail | Easy | Hidden | HardRock | SuddenDeath | Flashlight | FadeIn | Relax | Relax2 | SpunOut | KeyMod,
    ScoreIncreaseMods = Hidden | HardRock | DoubleTime | Flashlight | FadeIn
}

public class ModsExtensions
{
    public static readonly Dictionary<string, Mods> ModAliases = new(StringComparer.OrdinalIgnoreCase)
    {
        ["NF"]  = Mods.NoFail,
        ["EZ"]  = Mods.Easy,
        ["TD"]  = Mods.TouchDevice,
        ["HD"]  = Mods.Hidden,
        ["HR"]  = Mods.HardRock,
        ["SD"]  = Mods.SuddenDeath,
        ["DT"]  = Mods.DoubleTime,
        ["RX"]  = Mods.Relax,
        ["HT"]  = Mods.HalfTime,
        ["NC"]  = Mods.Nightcore,
        ["FL"]  = Mods.Flashlight,
        ["AT"]  = Mods.Autoplay,
        ["SO"]  = Mods.SpunOut,
        ["AP"]  = Mods.Relax2,
        ["PF"]  = Mods.Perfect,
        ["FI"]  = Mods.FadeIn,
        ["RD"]  = Mods.Random,
        ["CN"]  = Mods.Cinema,
        ["TP"]  = Mods.Target,
        ["V2"]  = Mods.ScoreV2,
        ["MR"]  = Mods.Mirror,
        // pełne nazwy
        ["NoFail"]      = Mods.NoFail,
        ["Easy"]        = Mods.Easy,
        ["Hidden"]      = Mods.Hidden,
        ["HardRock"]    = Mods.HardRock,
        ["SuddenDeath"] = Mods.SuddenDeath,
        ["DoubleTime"]  = Mods.DoubleTime,
        ["Relax"]       = Mods.Relax,
        ["HalfTime"]    = Mods.HalfTime,
        ["Nightcore"]   = Mods.Nightcore,
        ["Flashlight"]  = Mods.Flashlight,
        ["SpunOut"]     = Mods.SpunOut,
        ["Perfect"]     = Mods.Perfect,
        ["FadeIn"]      = Mods.FadeIn,
        ["Random"]      = Mods.Random,
        ["Cinema"]      = Mods.Cinema,
        ["ScoreV2"]     = Mods.ScoreV2,
        ["Mirror"]      = Mods.Mirror,
    };
    
    public static Mods ParseMods(IEnumerable<string> modStrings)
        => modStrings.Aggregate(Mods.None, (acc, m) => ModAliases.TryGetValue(m.Trim(), out var mod) ? acc | mod : acc);
}