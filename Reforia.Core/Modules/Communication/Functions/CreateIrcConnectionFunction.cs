using Microsoft.Extensions.DependencyInjection;
using Reforia.Core.Common;
using Reforia.Core.Common.Config.Contracts;
using Reforia.Core.Common.Config.Interfaces;
using Reforia.Core.Modules.Communication.Core;
using Reforia.Core.Modules.Communication.Exceptions;
using Reforia.Core.Modules.Communication.Functions.Response;
using Reforia.Core.Modules.Irc;
using Reforia.Core.Modules.Irc.Interfaces;
using Serilog;

namespace Reforia.Core.Modules.Communication.Functions;

public class CreateIrcConnectionFunction : WebFunction<CreateIrcConnectionFunctionResponse>
{
    protected override async Task<CreateIrcConnectionFunctionResponse> Handle(IServiceProvider provider)
    {
        var config = provider.GetService<IConfigService>();
        if (config == null)
            throw new NullReferenceException(nameof(IConfigService));

        var manager = provider.GetRequiredService<IrcConnectionManager>();

        var username = await config.Get(EConfigOptions.IrcUsername);

        var password = await config.Get(EConfigOptions.IrcPassword);

        if (username is null || password is null)
            throw new CommunicationException(EErrorCode.CannotGetIrcCredentials, "IRC credentials are missing");

        username = username.Replace(" ", "_");

        if (username.Equals("my_angel_cossin", StringComparison.CurrentCultureIgnoreCase))
            throw new CommunicationException(EErrorCode.UserNotAllowedToConnect, "User is banned");

        var connectionId = Guid.NewGuid().ToString();

        IrcConnection connection;

        try
        {
            if (manager.TryGetByUser(username, out var userConnection))
                await userConnection.DisposeAsync();

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