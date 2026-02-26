using Reforia.Rpc.Contracts;
using Serilog;

namespace Reforia.Rpc.Core;

public class WebDispatcher
{
    private readonly WebFunctionRegistry _registry;
    private readonly IServiceProvider _provider;

    public WebDispatcher(WebFunctionRegistry registry, IServiceProvider provider)
    {
        _registry = registry;
        _provider = provider;
    }
    
    public async Task<WebResponse> Dispatch(WebRequest request)
    {
        try
        {
            Log.Debug("Resolving function {FunctionName} for request {RequestId}", request.FunctionName, request.RequestId);
            var function = _registry.Resolve(request.FunctionName, _provider);

            var response = await function.Execute(request.Body, _provider);

            response.RequestId = request.RequestId;
            Log.Information("Completed request {RequestId} for {FunctionName}", request.RequestId, request.FunctionName);

            return response;
        }
        catch (Exception e)
        {
            Log.Error(e, "Failed to dispatch request {RequestId} for {FunctionName}", request.RequestId, request.FunctionName);
            return WebResponse.Error(request.RequestId, new List<string> { e.Message });
        }
    }
}