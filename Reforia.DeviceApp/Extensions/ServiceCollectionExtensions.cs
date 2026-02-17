using Reforia.Infrastructure.LiteDb;
using Reforia.Infrastructure.LiteDb.Interfaces;
using Reforia.IrcModule.Core;
using Reforia.Shared.Config;
using ReforiaBackend.Utils;

namespace ReforiaBackend.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddReforiaBackend(this IServiceCollection services)
    {
        services.AddSingleton<IIrcConnectionObserver, IrcSignalRBridge>();
        services.AddSingleton<IModuleDatabaseFactory, ModuleDatabaseFactory>();
        
        services.AddScoped<IConfigService, ConfigService>();
        services.AddScoped<IRepository<ConfigItem>>(sp =>
        {
            var factory = sp.GetRequiredService<IModuleDatabaseFactory>();
            var db = factory.GetDatabase("Config");
            return new DbRepository<ConfigItem>(db);
        });
    }
}