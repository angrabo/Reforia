using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Reforia.Rpc.Core;

namespace Reforia.Rpc;

public sealed class RpcBuilder
{
    public IServiceCollection Services { get; }

    internal RpcBuilder(IServiceCollection services)
    {
        Services = services;
    }
    
    public RpcBuilder AddFunctionsFromAssembly(Assembly assembly)
    {
        Services.Scan(scan => scan
                          .FromAssemblies(assembly)
                          .AddClasses(c => c.AssignableTo<IWebFunction>())
                          .AsSelf()
                          .AsImplementedInterfaces()
                          .WithScopedLifetime());

        return this;
    }
}