using System.Text.Json.Serialization;

namespace Reforia.Core.Modules.Communication.Functions.Body;

public class GetTestFunctionRequest
{
    [JsonRequired]
    public int Dummy { get; set; }
}