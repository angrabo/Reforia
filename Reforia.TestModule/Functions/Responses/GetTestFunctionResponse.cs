using Reforia.Rpc.Contracts;

namespace TestModule.Functions.Responses;

public class GetTestFunctionResponse : FunctionResponse
{
    public List<string> Players { get; set; } = new();
}