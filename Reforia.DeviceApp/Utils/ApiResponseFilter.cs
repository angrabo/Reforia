using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Reforia.Core.Common;
using ReforiaBackend.Dto;

namespace ReforiaBackend.Utils;

public class ApiResponseFilter : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (context.Result is ObjectResult objectResult)
        {
            if (objectResult.Value is IApiResponse)
            {
                await next();
                return;
            }

            var statusCode = objectResult.StatusCode ?? StatusCodes.Status200OK;
            
            context.Result = new ObjectResult(new ApiResponse<object>
            {
                StatusCode = statusCode,
                Data = objectResult.Value,
                ErrorCode = EErrorCode.None
            })
            {
                StatusCode = statusCode
            };
        }

        await next();
    }
}