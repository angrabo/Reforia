using Reforia.Rpc.Contracts;

namespace Reforia.IrcModule.Functions.Responses;

public class SendIrcMessageFunctionResponse : FunctionResponse
{
    public string ConnectionId { get; set; } = string.Empty;
    public bool Success { get; set; } = true;
}