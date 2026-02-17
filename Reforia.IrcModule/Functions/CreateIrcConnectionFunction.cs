using Microsoft.Extensions.DependencyInjection;
using Reforia.IrcModule.Core;
using Reforia.IrcModule.Functions.Body;
using Reforia.IrcModule.Functions.Response;
using Reforia.Rpc.Core;

namespace Reforia.IrcModule.Functions;

public class CreateIrcConnectionFunction : WebFunction<CreateIrcConnectionFunctionBody, CreateIrcConnectionFunctionResponse>
{
    protected override async Task<CreateIrcConnectionFunctionResponse> Handle(CreateIrcConnectionFunctionBody body, IServiceProvider provider)
    {
        var connectionId = Guid.NewGuid().ToString();
        
        var manager = provider.GetRequiredService<IrcConnectionManager>();

        var connection = await manager.CreateAsync(
                             connectionId,
                             body.Host,
                             body.Port,
                             body.Nick,
                             body.Password);

        var observers = provider.GetServices<IIrcConnectionObserver>();
        
        foreach (var observer in observers)
            await observer.AttachAsync(connection);

        return new CreateIrcConnectionFunctionResponse
        {
            ConnectionId = connectionId
        };
    }
}