using Reforia.Rpc.Core;
using TestModule.Functions.Response;

namespace TestModule.Functions;

public class GetTestFunction2 : WebFunction<GetTestFunctionResponse>
{
    protected async override Task<GetTestFunctionResponse> Handle(IServiceProvider provider)
    {
        return new GetTestFunctionResponse()
        {
            Players = ["meow"]
        };
    }
}