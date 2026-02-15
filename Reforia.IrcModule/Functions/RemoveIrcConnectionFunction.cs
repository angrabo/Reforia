using Microsoft.Extensions.DependencyInjection;
using Reforia.IrcModule.Core;
using Reforia.IrcModule.Functions.Body;
using Reforia.IrcModule.Functions.Response;
using Reforia.Rpc.Core;

namespace Reforia.IrcModule.Functions;

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