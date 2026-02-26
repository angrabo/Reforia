using Reforia.Core.Common;

namespace Reforia.Core.Modules.Communication.Contracts;

public class WebResponse
{
    public string RequestId { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public object? Body { get; set; }
    public ErrorCode ErrorCode { get; set; }
    public string Details { get; set; } = string.Empty;
    
    public static WebResponse Ok(string requestId, object? body) 
        => new() { RequestId = requestId, StatusCode = 200, Body = body, ErrorCode = ErrorCode.None};
    
    public static WebResponse BadRequest(string requestId, ErrorCode error = ErrorCode.Unknown, string details = "")
        => new() { RequestId = requestId, StatusCode = 400, ErrorCode = error, Details = details };
    
    public static WebResponse Error(string requestId, ErrorCode error = ErrorCode.Unknown, string details = "")
        => new() { RequestId = requestId, StatusCode = 500, ErrorCode = error, Details = details}; 
}