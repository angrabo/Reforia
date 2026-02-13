using Microsoft.Extensions.Logging;
using Reforia.Rpc.Contracts;

namespace Reforia.Rpc.Core;

public class WebDispatcher
{
    private readonly WebFunctionRegistry _registry;
    private readonly IServiceProvider _provider;
    private readonly ILogger<WebDispatcher> _logger;

    public WebDispatcher(WebFunctionRegistry registry, IServiceProvider provider, ILogger<WebDispatcher> logger)
    {
        _registry = registry;
        _provider = provider;
        _logger = logger;
    }
    
    public async Task<WebResponse> Dispatch(WebRequest request)
    {
        try
        {
            _logger.LogDebug("Resolving function {FunctionName} for request {RequestId}", request.FunctionName, request.RequestId);
            var function = _registry.Resolve(request.FunctionName, _provider);

            var response = await function.Execute(request.Body, _provider);

            response.RequestId = request.RequestId;
            _logger.LogInformation("Completed request {RequestId} for {FunctionName}", request.RequestId, request.FunctionName);

            return response;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to dispatch request {RequestId} for {FunctionName}", request.RequestId, request.FunctionName);
            return WebResponse.Error(request.RequestId, new List<string> { e.Message });
        }
    }
}