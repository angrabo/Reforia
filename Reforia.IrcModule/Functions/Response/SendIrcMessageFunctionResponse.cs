using Reforia.Rpc.Contracts;

namespace Reforia.IrcModule.Functions.Response;

public class SendIrcMessageFunctionResponse
{
    public string ConnectionId { get; set; } = string.Empty;
    public bool Success { get; set; } = true;
}