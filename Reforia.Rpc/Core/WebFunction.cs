using Reforia.Rpc.Contracts;
using System.Text.Json;
using Serilog;

namespace Reforia.Rpc.Core;

public abstract class WebFunction<TBody, TResponse> : IWebFunction 
    where TBody : class 
    where TResponse : class
{
    public string Name => GetType().Name;

    public async Task<WebResponse> Execute(string jsonBody, IServiceProvider provider)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(jsonBody))
                return WebResponse.BadRequest("", new List<string> { "Body is required for this function." });

            var body = JsonSerializer.Deserialize<TBody>(jsonBody, 
                                                               new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (body == null)
                return WebResponse.BadRequest("", new List<string> { "Invalid request body" });

            var result = await Handle(body, provider);
            return WebResponse.Ok("", result);
        }
        catch (Exception e)
        {
            Log.Error(e, "Error executing {FunctionName}", Name);
            return WebResponse.Error("", new List<string> { e.Message });
        }
    }

    protected abstract Task<TResponse> Handle(TBody request, IServiceProvider provider);
}

public abstract class WebFunction<TResponse> : IWebFunction 
    where TResponse : class
{
    public string Name => GetType().Name;

    public async Task<WebResponse> Execute(string jsonBody, IServiceProvider provider)
    {
        try
        {
            var result = await Handle(provider);
            return WebResponse.Ok("", result);
        }
        catch (Exception e)
        {
            Log.Error(e, "Error executing {FunctionName}", Name);
            return WebResponse.Error("", new List<string> { e.Message });
        }
    }

    protected abstract Task<TResponse> Handle(IServiceProvider provider);
}