namespace Reforia.RpcTestTool;

public sealed class WebResponse
{
    public string RequestId { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public object? Body { get; set; }
    public List<string>? Errors { get; set; }
}