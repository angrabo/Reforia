using Microsoft.Extensions.DependencyInjection;
using Reforia.Rpc;
using TestModule.Services;

namespace TestModule;

public static class TestNoduleExtensions
{
    public static RpcBuilder AddTourneyModule(this RpcBuilder builder)
    {
        builder.Services.AddScoped<ITestService, TestService>();

        builder.AddFunctionsFromAssembly(typeof(TestNoduleExtensions).Assembly);

        return builder;
    }
}