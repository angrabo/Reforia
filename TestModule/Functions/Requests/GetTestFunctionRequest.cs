using System.Text.Json.Serialization;

namespace ReforiaBackend.Rpc.Functions.Requests;

public class GetTestFunctionRequest
{
    [JsonRequired]
    public int Dummy { get; set; }
}