namespace Reforia.Core.Modules.Communication.Functions.Body;

public class SetIrcCredentialsFunctionBody
{
    public required string Nick { get; set; }
    public string Password { get; set; } = string.Empty;
}