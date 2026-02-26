using System.Text.Json;
using Reforia.Rpc.Contracts;

namespace Reforia.Rpc.Core;

public abstract class WebFunction<TRequest, TResponse> : IWebFunction where TRequest : FunctionBody where TResponse : FunctionResponse
{
    public string Name => GetType().Name;
    
    public async Task<WebResponse> Execute(string jsonBody, IServiceProvider provider)
    {
        try
        {
            var request = JsonSerializer.Deserialize<TRequest>(jsonBody, new JsonSerializerOptions( ) { PropertyNameCaseInsensitive = true});
            
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
    
    protected abstract Task<TResponse> Handle(TRequest body, IServiceProvider provider);
}