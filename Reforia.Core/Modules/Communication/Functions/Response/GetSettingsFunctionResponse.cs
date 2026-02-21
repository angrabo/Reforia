namespace Reforia.Core.Modules.Communication.Functions.Response;

public class GetSettingsFunctionResponse
{
    public string ApiToken { get; set; } = string.Empty;
    public bool IsUserHighlighted { get; set; }
}