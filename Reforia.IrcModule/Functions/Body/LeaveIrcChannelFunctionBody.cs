namespace Reforia.IrcModule.Functions.Body;

public class LeaveIrcChannelFunctionBody
{
    public required string ConnectionId { get; set; }
    public required string Channel { get; set; }
}