using Reforia.Core.Modules.Irc.Interfaces;
using Reforia.Core.Utils;
using ReforiaBackend.Utils;

namespace ReforiaBackend.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddReforiaBackend(this IServiceCollection services, ServicesOptionsModel options)
    {
        services.AddSingleton<IIrcConnectionObserver, IrcSignalRBridge>();
        services.AddHttpClient();
        services.AddRequiredServices(options);        
        
    }
}