using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ReforiaBackend.Dto;

namespace ReforiaBackend.Utils;

public class ApiResponseFilter : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(
        ResultExecutingContext context,
        ResultExecutionDelegate next)
    {
        if (context.Result is ObjectResult objectResult)
        {
            var statusCode = objectResult.StatusCode ?? StatusCodes.Status200OK;

            context.Result = new ObjectResult(new ApiResponse<object>
            {
                StatusCode = statusCode,
                Data = objectResult.Value
            })
            {
                StatusCode = statusCode
            };
        }

        await next();
    }
}