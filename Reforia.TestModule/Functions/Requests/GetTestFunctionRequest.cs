using System.Text.Json.Serialization;
using Reforia.Rpc.Contracts;

namespace TestModule.Functions.Requests;

public class GetTestFunctionRequest : FunctionBody
{
    [JsonRequired]
    public int Dummy { get; set; }
}