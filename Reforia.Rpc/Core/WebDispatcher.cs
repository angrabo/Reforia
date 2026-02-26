namespace ReforiaBackend.Rpc.Core;

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
            var function = _registry.Resolve(request.FunctionName, _provider);
            
            var response = await function.Execute(request.Body, _provider);

            response.RequestId = request.RequestId;

            return response;
        }
        catch (Exception e)
        {
            //TODO: log
            return WebResponse.Error(request.RequestId, new List<string> { e.Message });
        }
    }
}