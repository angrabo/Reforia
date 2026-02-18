using Microsoft.Extensions.DependencyInjection;
using Reforia.Core.Modules.Communication.Core;
using Reforia.Core.Modules.Communication.Functions.Body;
using Reforia.Core.Modules.Communication.Functions.Response;
using Reforia.Core.Modules.Irc;
using Reforia.Core.Modules.Irc.Interfaces;

namespace Reforia.Core.Modules.Communication.Functions;

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