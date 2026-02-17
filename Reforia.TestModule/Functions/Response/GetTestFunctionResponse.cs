using Reforia.Rpc.Contracts;

namespace TestModule.Functions.Response;

public class GetTestFunctionResponse
{
    public List<string> Players { get; set; } = new();
}