using Microsoft.Extensions.DependencyInjection;
using Reforia.Core.Modules.Communication.Core;
using Reforia.Core.Modules.Communication.Functions.Body;
using Reforia.Core.Modules.Communication.Functions.Response;
using Reforia.Core.Modules.Test.Services;

namespace Reforia.Core.Modules.Communication.Functions;

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