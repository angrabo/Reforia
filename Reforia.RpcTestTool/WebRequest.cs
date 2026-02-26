namespace Reforia.RpcTestTool;

public sealed class WebRequest
{
    public string FunctionName { get; set; } = string.Empty;
    public string RequestId { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}