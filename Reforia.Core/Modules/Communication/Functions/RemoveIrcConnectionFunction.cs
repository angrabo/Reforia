using Microsoft.Extensions.DependencyInjection;
using Reforia.Core.Modules.Communication.Core;
using Reforia.Core.Modules.Communication.Functions.Body;
using Reforia.Core.Modules.Communication.Functions.Response;
using Reforia.Core.Modules.Irc;

namespace Reforia.Core.Modules.Communication.Functions;

public class RemoveIrcConnectionFunction : WebFunction<RemoveIrcConnectionFunctionBody, RemoveIrcConnectionFunctionResponse>
{
    protected override async Task<RemoveIrcConnectionFunctionResponse> Handle(RemoveIrcConnectionFunctionBody body, IServiceProvider provider)
    {
        var manager = provider.GetService<IrcConnectionManager>();
        if (manager is null)
            throw new Exception("IrcManager not found");

        if (!manager.TryGet(body.ConnectionId, out var connection))
            throw new Exception("Connection not found");
        
        await connection.DisposeAsync();
        
        return new RemoveIrcConnectionFunctionResponse()
        {
            Success = true
        };
    }
}