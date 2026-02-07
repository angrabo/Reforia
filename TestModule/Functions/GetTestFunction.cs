using Reforia.Rpc.Core;
using ReforiaBackend.Rpc.Functions.Requests;
using ReforiaBackend.Rpc.Functions.Responses;

namespace ReforiaBackend.Rpc.Functions;

public class GetTestFunction : WebFunction<GetTestFunctionRequest, GetTestFunctionResponse>
{
    protected override async Task<GetTestFunctionResponse> Handle(GetTestFunctionRequest request, IServiceProvider provider)
    {
        var testService = provider.GetRequiredService<ITestService>();
        
        return new GetTestFunctionResponse
        {
            Players = await testService.GetAll()
        };
    }
}