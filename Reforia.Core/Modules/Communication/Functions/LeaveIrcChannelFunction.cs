using Microsoft.Extensions.DependencyInjection;
using Reforia.Core.Modules.Communication.Core;
using Reforia.Core.Modules.Communication.Functions.Body;
using Reforia.Core.Modules.Communication.Functions.Response;
using Reforia.Core.Modules.Irc;

namespace Reforia.Core.Modules.Communication.Functions;

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