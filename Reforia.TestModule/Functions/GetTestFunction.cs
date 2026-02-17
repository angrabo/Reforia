using Microsoft.Extensions.DependencyInjection;
using Reforia.Rpc.Core;
using TestModule.Functions.Body;
using TestModule.Functions.Response;
using TestModule.Services;

namespace TestModule.Functions;

public class GetTestFunction : WebFunction<GetTestFunctionRequest, GetTestFunctionResponse>
{
    protected override async Task<GetTestFunctionResponse> Handle(GetTestFunctionRequest body, IServiceProvider provider)
    {
        var testService = provider.GetRequiredService<ITestService>();

        return new GetTestFunctionResponse
        {
            Players = await testService.GetAll()
        };
    }
}