using Microsoft.Extensions.DependencyInjection;
using Reforia.Core.Common;
using Reforia.Core.Common.Config.Interfaces;
using Reforia.Core.Modules.Communication.Contracts;
using Reforia.Core.Modules.Communication.Core;
using Reforia.Core.Modules.Communication.Exceptions;
using Reforia.Core.Modules.Communication.Functions.Body;
using Reforia.Core.Modules.Communication.Functions.Response;
using Reforia.Core.Modules.Irc;
using Reforia.Core.Modules.Irc.Interfaces;
using Serilog;

namespace Reforia.Core.Modules.Communication.Functions;

// public class CreateIrcConnectionFunction : WebFunction<CreateIrcConnectionFunctionBody, CreateIrcConnectionFunctionResponse>
// {
//     protected override async Task<CreateIrcConnectionFunctionResponse> Handle(CreateIrcConnectionFunctionBody body, IServiceProvider provider)
//     {
//         var connectionId = Guid.NewGuid().ToString();
//         
//         var manager = provider.GetRequiredService<IrcConnectionManager>();
//
//         var connection = await manager.CreateAsync(
//                              connectionId,
//                              body.Host,
//                              body.Port,
//                              body.Nick,
//                              body.Password);
//
//         var observers = provider.GetServices<IIrcConnectionObserver>();
//         
//         foreach (var observer in observers)
//             await observer.AttachAsync(connection);
//
//         return new CreateIrcConnectionFunctionResponse
//         {
//             ConnectionId = connectionId
//         };
//     }
// }

public class CreateIrcConnectionFunction : WebFunction<CreateIrcConnectionFunctionResponse>
{
    protected override async Task<CreateIrcConnectionFunctionResponse> Handle(IServiceProvider provider)
    {
        var config = provider.GetService<IConfigService>();
        if (config == null)
            throw new NullReferenceException(nameof(IConfigService));
        
        var manager = provider.GetRequiredService<IrcConnectionManager>();

        var username = await config.Get(EConfigOptions.IrcUsername);

        if (username == "My Angel Cossin")
            throw new CommunicationException(EErrorCode.UserNotAllowedToConnect, "User is banned");
        
        var password = await config.Get(EConfigOptions.IrcPassword);
        
        if (username is null || password is null)
            throw new CommunicationException(EErrorCode.CannotGetIrcCredentials, "IRC credentials are missing");
        
        var connectionId = Guid.NewGuid().ToString();
        
        IrcConnection connection;
        
        try
        {
            connection = await manager.CreateAsync(
                                 connectionId,
                                 "irc.ppy.sh",
                                 6667,
                                 username,
                                 password);
        }
        catch (Exception e)
        {
            Log.Warning(e, "Failed to create IRC connection with id {ConnectionId}", connectionId);
            throw new CommunicationException(EErrorCode.CannotConnectToIrc, "Invalid IRC credentials or network error");
        }
        
        var observers = provider.GetServices<IIrcConnectionObserver>();
        foreach (var observer in observers)
            await observer.AttachAsync(connection);

        return new CreateIrcConnectionFunctionResponse()
        {
            ConnectionId = connectionId,
        };
    }
}