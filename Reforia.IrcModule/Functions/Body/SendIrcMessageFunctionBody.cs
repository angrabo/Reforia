using Reforia.Rpc.Contracts;

namespace Reforia.IrcModule.Functions.Body;

public class SendIrcMessageFunctionBody : FunctionBody
{
    public string Channel { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string ConnectionId { get; set; } = string.Empty;
}