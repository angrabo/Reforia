using System.Text.Json.Serialization;
using Reforia.Rpc.Contracts;

namespace TestModule.Functions.Body;

public class GetTestFunctionRequest
{
    [JsonRequired]
    public int Dummy { get; set; }
}