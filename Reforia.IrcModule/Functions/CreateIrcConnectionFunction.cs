using Microsoft.Extensions.DependencyInjection;
using Reforia.IrcModule.Core;
using Reforia.IrcModule.Functions.Request;
using Reforia.IrcModule.Functions.Responses;
using Reforia.Rpc.Core;

namespace Reforia.IrcModule.Functions;

public class CreateIrcConnectionFunction : WebFunction<CreateIrcConnectionFunctionRquest, CreateIrcConnectionFunctionResponse>
{
    protected override async Task<CreateIrcConnectionFunctionResponse> Handle(CreateIrcConnectionFunctionRquest request, IServiceProvider provider)
    {
        
        var connectionId = Guid.NewGuid().ToString();
        
        var manager = provider.GetRequiredService<IrcConnectionManager>();

        var connection = await manager.CreateAsync(
                             connectionId,
                             request.Host,
                             request.Port,
                             request.Nick,
                             request.Password);

        var observers = provider.GetServices<IIrcConnectionObserver>();
        
        foreach (var observer in observers)
            await observer.AttachAsync(connection);

        return new CreateIrcConnectionFunctionResponse
        {
            ConnectionId = connectionId
        };
    }
}