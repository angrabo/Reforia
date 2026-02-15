using Microsoft.Extensions.DependencyInjection;
using Reforia.IrcModule.Core;
using Reforia.IrcModule.Functions.Body;
using Reforia.IrcModule.Functions.Response;
using Reforia.Rpc.Core;

namespace Reforia.IrcModule.Functions;

public class JoinIrcChannelFunction : WebFunction<JoinIrcChannelFunctionBody, JoinIrcChannelFunctionResponse>
{
    protected override async Task<JoinIrcChannelFunctionResponse> Handle(JoinIrcChannelFunctionBody body, IServiceProvider provider)
    {
        var manager = provider.GetService<IrcConnectionManager>();
        if (manager is null)
            throw new Exception("IrcManager not found");

        if (!manager.TryGet(body.ConnectionId, out var connection))
            throw new Exception("Connection not found");

        var success = await connection.JoinChannelAsync(body.Channel);

        return new JoinIrcChannelFunctionResponse()
        {
            Success = success
        };
    }
}