using ReforiaBackend.Rpc.Core;
using ReforiaBackend.Services;

namespace ReforiaBackend.Extensions;

public static class ServiceCollectionExtensions
{

    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton<ITestService, TestService>();
    }
    
    public static void AddRpcFunctions(this IServiceCollection services)
    {
        services.AddScoped<WebFunctionRegistry>();
        services.AddScoped<WebDispatcher>();

        services.Scan(scan => scan
                          .FromAssemblyOf<IWebFunction>()
                          .AddClasses(c => c.AssignableTo<IWebFunction>())
                          .AsSelf()
                          .AsImplementedInterfaces()
                          .WithScopedLifetime());
    }
}