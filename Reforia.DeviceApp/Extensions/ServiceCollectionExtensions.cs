using Reforia.Core.Modules.Irc.Interfaces;
using Reforia.Core.Utils;
using ReforiaBackend.Utils;

namespace ReforiaBackend.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddReforiaBackend(this IServiceCollection services)
    {
        services.AddSingleton<IIrcConnectionObserver, IrcSignalRBridge>();
        services.AddRequiredServices(new ServicesOptionsModel()
        {
            DatabseConnectionString = "Data Source=reforia.db"
        });        
    }
}