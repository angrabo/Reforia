namespace Reforia.Rpc.Contracts;

public class WebResponse
{
    public string RequestId { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public object? Body { get; set; }
    public List<string>? Errors { get; set; }
    
    public static WebResponse Ok(object? body) 
        => new() { StatusCode = 200, Body = body, Errors = []};
    
    public static WebResponse BadRequest(List<string> errors) 
        => new() { StatusCode = 403, Errors = errors };
    
    public static WebResponse Error(List<string> errors) 
        => new() { StatusCode = 500, Errors = errors };
    
    public static WebResponse Ok(string requestId, object? body) 
        => new() { RequestId = requestId, StatusCode = 200, Body = body, Errors = []};
    
    public static WebResponse BadRequest(string requestId, List<string> errors)
        => new() { RequestId = requestId, StatusCode = 400, Errors = errors };
    
    public static WebResponse Error(string requestId, List<string> errors)
        => new() { RequestId = requestId, StatusCode = 500, Errors = errors }; 
}