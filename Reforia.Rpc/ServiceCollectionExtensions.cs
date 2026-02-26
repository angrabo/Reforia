using Microsoft.Extensions.DependencyInjection;
using Reforia.Rpc.Core;

namespace Reforia.Rpc;

public static class ServiceCollectionExtensions
{
    public static RpcBuilder AddRpc(this IServiceCollection services)
    {
        services.AddScoped<WebFunctionRegistry>();
        services.AddScoped<WebDispatcher>();

        return new RpcBuilder(services);
    }
}