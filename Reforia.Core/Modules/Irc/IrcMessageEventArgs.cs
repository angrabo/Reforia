namespace Reforia.Core.Modules.Irc;

public record IrcMessageEventArgs
{
    public string ConnectionId { get; init; }
    public string RawMessage { get; init; }
}