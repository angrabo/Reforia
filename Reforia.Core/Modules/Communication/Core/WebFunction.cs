using System.Text.Json;
using Reforia.Core.Common;
using Reforia.Core.Modules.Communication.Contracts;
using Reforia.Core.Modules.Communication.Exceptions;
using Reforia.Core.Modules.Communication.Interfaces;
using Serilog;

namespace Reforia.Core.Modules.Communication.Core;

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
                return WebResponse.BadRequest("", EErrorCode.CannotDeserializeRequest,
                                              "Body is required for this function.");

            var body = JsonSerializer.Deserialize<TBody>(jsonBody,
                                                         new JsonSerializerOptions
                                                             { PropertyNameCaseInsensitive = true });

            if (body == null)
                return WebResponse.BadRequest("", EErrorCode.CannotDeserializeRequest);

            var result = await Handle(body, provider);
            return WebResponse.Ok("", result);
        }
        catch (JsonException e)
        {
            return WebResponse.BadRequest("", EErrorCode.CannotDeserializeRequest, e.Message);
        }
        catch (CommunicationException e)
        {
            Log.Error(e, "Error executing {FunctionName}", Name);
            return WebResponse.Error("", e.EErrorCode, e.Message);
        }
        catch (Exception e)
        {
            Log.Error(e, "Error executing {FunctionName}", Name);
            return WebResponse.Error("", EErrorCode.Unknown, e.Message);
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
        catch (CommunicationException e)
        {
            Log.Error(e, "Error executing {FunctionName}", Name);
            return WebResponse.Error("", e.EErrorCode, e.Message);
        }
        catch (Exception e)
        {
            Log.Error(e, "Error executing {FunctionName}", Name);
            return WebResponse.Error("", EErrorCode.ErrorDuringFunctionExecute, e.Message);
        }
    }

    protected abstract Task<TResponse> Handle(IServiceProvider provider);
}