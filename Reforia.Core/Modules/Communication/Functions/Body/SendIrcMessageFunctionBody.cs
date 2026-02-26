namespace Reforia.Core.Modules.Communication.Functions.Body;

public class SendIrcMessageFunctionBody
{
    public string Channel { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string ConnectionId { get; set; } = string.Empty;
}