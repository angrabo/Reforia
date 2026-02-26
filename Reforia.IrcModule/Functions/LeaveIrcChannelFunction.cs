using Microsoft.Extensions.DependencyInjection;
using Reforia.IrcModule.Core;
using Reforia.IrcModule.Functions.Body;
using Reforia.IrcModule.Functions.Response;
using Reforia.Rpc.Core;


namespace Reforia.IrcModule.Functions;

public class LeaveIrcChannelFunction : WebFunction<LeaveIrcChannelFunctionBody, LeaveIrcChannelFunctionResponse>
{
    protected override async Task<LeaveIrcChannelFunctionResponse> Handle(LeaveIrcChannelFunctionBody body, IServiceProvider provider)
    {
        var manager = provider.GetService<IrcConnectionManager>();
        if (manager is null)
            throw new Exception("IrcManager not found");

        if (!manager.TryGet(body.ConnectionId, out var connection))
            throw new Exception("Connection not found");

        var success = await connection.LeaveChannelAsync(body.Channel[1..]);

        return new LeaveIrcChannelFunctionResponse()
        {
            Success = success,
        };
    }
}