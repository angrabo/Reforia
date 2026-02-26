using System.Text.Json.Serialization;

namespace TestModule.Functions.Requests;

public class GetTestFunctionRequest
{
    [JsonRequired]
    public int Dummy { get; set; }
}