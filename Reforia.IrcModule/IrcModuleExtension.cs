using Microsoft.Extensions.DependencyInjection;
using Reforia.IrcModule.Core;
using Reforia.Rpc;

namespace Reforia.IrcModule;

public static class IrcModuleExtensions
{
    public static RpcBuilder AddIrcModule(this RpcBuilder builder)
    {
        builder.Services.AddSingleton<IrcConnectionManager>();
        
        builder.AddFunctionsFromAssembly(typeof(IrcModuleExtensions).Assembly);

        return builder;
    }
}