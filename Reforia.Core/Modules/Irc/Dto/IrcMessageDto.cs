namespace Reforia.Core.Modules.Irc.Dto;

public class IrcMessageDto
{
    public string ConnectionId { get; init; } = null!;
    public string ChatId { get; init; } = null!;      
    public string Sender { get; init; } = null!;
    public string Message { get; init; } = null!;
    public DateTime Timestamp { get; init; }
}