using Microsoft.Extensions.DependencyInjection;
using Reforia.Infrastructure.LiteDb;
using Reforia.IrcModule.Core;
using Reforia.Rpc;
using Reforia.Shared.Config;
using Serilog.Core;

namespace Reforia.IrcModule;

public static class IrcModuleExtensions
{
    public static void AddIrcModule(this RpcBuilder builder)
    {
        ValidateRequiredServices(builder);
        
        builder.Services.AddSingleton<IrcConnectionManager>();
        
        builder.AddFunctionsFromAssembly(typeof(IrcModuleExtensions).Assembly);
    }

    private static void ValidateRequiredServices(RpcBuilder builder)
    {
        if (builder.Services.All(descriptor => descriptor.ServiceType != typeof(IConfigService))) 
            throw new InvalidOperationException($"{nameof(IConfigService)} must be registered in IrcModule");
    }
}