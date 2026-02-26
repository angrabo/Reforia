using Microsoft.Extensions.DependencyInjection;
using Reforia.Core.Modules.Communication.Core;
using Reforia.Core.Modules.Communication.Functions.Body;
using Reforia.Core.Modules.Communication.Functions.Response;
using Reforia.Core.Modules.Irc;

namespace Reforia.Core.Modules.Communication.Functions;

public class SendIrcMessageFunction : WebFunction<SendIrcMessageFunctionBody, SendIrcMessageFunctionResponse>
{
    protected override async Task<SendIrcMessageFunctionResponse> Handle(SendIrcMessageFunctionBody body, IServiceProvider provider)
    {
        bool success;
        
        var manager = provider.GetService<IrcConnectionManager>();
        if (manager is null)
            throw new Exception("IrcManager not found");

        if (!manager.TryGet(body.ConnectionId, out var connection))
            throw new Exception("Connection not found");
        
        if (body.Channel.Contains('#'))
            success = await connection.SendChannelMessageAsync(body.Message, body.Channel[1..]);
        else
            success = await connection.SendPrivateMessageAsync(body.Message, body.Channel);
        
        return new SendIrcMessageFunctionResponse()
        {
            Success = success,
            ConnectionId = body.ConnectionId
        };
    }
}