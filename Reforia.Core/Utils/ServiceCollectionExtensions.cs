using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Reforia.Core.Common.Config.Interfaces;
using Reforia.Core.Common.Config.Services;
using Reforia.Core.Common.Database;
using Reforia.Core.Common.Database.Interfaces;
using Reforia.Core.Modules.Communication.Core;
using Reforia.Core.Modules.Communication.Interfaces;
using Reforia.Core.Modules.Irc;

namespace Reforia.Core.Utils;

public static class ServiceCollectionExtensions
{
    public static void AddRequiredServices(this IServiceCollection services, ServicesOptionsModel options)
    {
        services.AddDatabase(options.DatabseConnectionString);
        services.AddConfigServices();
        services.AddIrcModule();
        services.AddCommunicationModule();
    }

    public static void AddConfigServices(this IServiceCollection services)
    {
        services.AddScoped<IConfigService, ConfigService>();
    }

    public static void AddIrcModule(this IServiceCollection services)
    {
        services.AddSingleton<IrcConnectionManager>();
    }

    public static void AddCommunicationModule(this IServiceCollection services)
    {
        services.AddScoped<WebFunctionRegistry>();
        services.AddScoped<WebDispatcher>();

        services.Scan(scan => scan
                          .FromAssemblies(Assembly.GetExecutingAssembly())
                          .AddClasses(c => c.AssignableTo<IWebFunction>())
                          .AsSelf()
                          .AsImplementedInterfaces()
                          .WithScopedLifetime());
    }

    public static void AddDatabase(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));

        services.Scan(scan => scan
                          .FromAssemblies(typeof(IRepository<>).Assembly)
                          .AddClasses(classes => classes.AssignableTo(typeof(IRepository<>)))
                          .AsImplementedInterfaces()
                          .WithScopedLifetime());
    }
}