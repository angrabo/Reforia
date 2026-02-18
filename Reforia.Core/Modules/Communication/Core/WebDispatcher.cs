using Reforia.Core.Common;
using Reforia.Core.Modules.Communication.Contracts;
using Serilog;

namespace Reforia.Core.Modules.Communication.Core;

public class WebDispatcher
{
    private readonly WebFunctionRegistry _registry;
    private readonly IServiceProvider _provider;

    public WebDispatcher(WebFunctionRegistry registry, IServiceProvider provider)
    {
        _registry = registry;
        _provider = provider;
    }
    
    public async Task<WebResponse> Dispatch(WebBody body)
    {
        try
        {
            Log.Debug("Resolving function {FunctionName} for request {RequestId}", body.FunctionName, body.RequestId);
            var function = _registry.Resolve(body.FunctionName, _provider);

            var response = await function.Execute(body.Body, _provider);

            response.RequestId = body.RequestId;
            Log.Information("Completed request {RequestId} for {FunctionName}", body.RequestId, body.FunctionName);

            return response;
        }
        catch (Exception e)
        {
            var message = $"Failed to dispatch request {body.RequestId} for {body.FunctionName}";
            Log.Error(e, message);
            return WebResponse.Error(body.RequestId, ErrorCode.FunctionNotFound, message);
        }
    }
}