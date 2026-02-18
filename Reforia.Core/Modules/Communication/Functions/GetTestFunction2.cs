using Reforia.Core.Modules.Communication.Core;
using Reforia.Core.Modules.Communication.Functions.Response;

namespace Reforia.Core.Modules.Communication.Functions;

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