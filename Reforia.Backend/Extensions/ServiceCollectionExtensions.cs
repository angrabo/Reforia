using Reforia.IrcModule.Core;
using ReforiaBackend.Utils;

namespace ReforiaBackend.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddReforiaBackend(this IServiceCollection services)
    {
        services.AddSingleton<IIrcConnectionObserver, IrcSignalRBridge>();
    }
}