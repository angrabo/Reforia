using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Reforia.Rpc.Contracts;
using System.Text.Json;

namespace Reforia.Rpc.Core;

public abstract class WebFunction<TRequest, TResponse> : IWebFunction
{
    public string Name => GetType().Name;
    public async Task<WebResponse> Execute(string jsonBody, IServiceProvider provider)
    {
        var logger = provider.GetService<ILogger<WebFunction<TRequest, TResponse>>>();
        try
        {
            logger?.LogInformation("Executing function {FunctionName}", Name);

            var request = JsonSerializer.Deserialize<TRequest>(jsonBody, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

            if (request == null)
            {
                logger?.LogWarning("Invalid request body for function {FunctionName}", Name);
                return WebResponse.BadRequest("", new List<string> { "Invalid request body" });
            }

            var result = await Handle(request, provider);

            logger?.LogInformation("Function {FunctionName} executed successfully", Name);

            return WebResponse.Ok("", result);
        }
        catch (Exception e)
        {
            logger?.LogError(e, "Error executing function {FunctionName}", Name);
            return WebResponse.Error("", new List<string> { e.Message });
        }
    }

    protected abstract Task<TResponse> Handle(TRequest request, IServiceProvider provider);
}