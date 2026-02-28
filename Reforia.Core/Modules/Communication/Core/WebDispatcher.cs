using Reforia.Core.Common;
using Reforia.Core.Modules.Communication.Contracts;
using Reforia.Core.Utils;

namespace Reforia.Core.Modules.Communication.Core;

public class WebDispatcher
{
    private readonly WebFunctionRegistry _registry;
    private readonly IServiceProvider    _provider;

    public WebDispatcher(WebFunctionRegistry registry, IServiceProvider provider)
    {
        _registry = registry;
        _provider = provider;
    }

    public async Task<WebResponse> Dispatch(WebRequest request)
    {
        try
        {
            Logger.Debug($"Resolving function {request.FunctionName} for request {request.RequestId}");
            var function = _registry.Resolve(request.FunctionName, _provider);

            var response = await function.Execute(request.Body, _provider);

            response.RequestId = request.RequestId;
            Logger.Info($"Completed request {request.RequestId} for {request.FunctionName}");

            return response;
        }
        catch (Exception e)
        {
            var message = $"Failed to dispatch request {request.RequestId} for {request.FunctionName}";
            Logger.Error(e, message);
            return WebResponse.Error(request.RequestId, EErrorCode.FunctionNotFound, message);
        }
    }
}