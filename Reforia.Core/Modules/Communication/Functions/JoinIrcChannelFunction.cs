using Microsoft.Extensions.DependencyInjection;
using Reforia.Core.Modules.Communication.Core;
using Reforia.Core.Modules.Communication.Functions.Body;
using Reforia.Core.Modules.Communication.Functions.Response;
using Reforia.Core.Modules.Irc;

namespace Reforia.Core.Modules.Communication.Functions;

public class JoinIrcChannelFunction : WebFunction<JoinIrcChannelFunctionBody, JoinIrcChannelFunctionResponse>
{
    protected override async Task<JoinIrcChannelFunctionResponse> Handle(JoinIrcChannelFunctionBody body, IServiceProvider provider)
    {
        var manager = provider.GetService<IrcConnectionManager>();
        if (manager is null)
            throw new Exception("IrcManager not found");

        if (!manager.TryGet(body.ConnectionId, out var connection))
            throw new Exception("Connection not found");

        var success = await connection.JoinChannelAsync(body.Channel[1..]);

        return new JoinIrcChannelFunctionResponse()
        {
            Success = success
        };
    }
}