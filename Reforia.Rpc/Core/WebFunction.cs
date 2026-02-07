using System.Text.Json;

namespace ReforiaBackend.Rpc.Core;

public abstract class WebFunction<TRequest, TResponse> : IWebFunction
{
    public string Name => GetType().Name;
    
    public async Task<WebResponse> Execute(string jsonBody, IServiceProvider provider)
    {
        try
        {
            var request = JsonSerializer.Deserialize<TRequest>(jsonBody);
            
            if (request == null)
                return WebResponse.BadRequest("", new List<string> { "Invalid request body" });
            
            var result = await Handle(request, provider);
            
            return WebResponse.Ok("", result);
        }
        catch (Exception e)
        {
            //TODO: log
            return WebResponse.Error("", new List<string> { e.Message });
        }
    }
    
    protected abstract Task<TResponse> Handle(TRequest request, IServiceProvider provider);
}