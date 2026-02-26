namespace Reforia.IrcModule.Core;

public record IrcMessageEventArgs
{
    public string ConnectionId { get; init; }
    public string RawMessage { get; init; }
}