namespace Reforia.IrcModule.Functions.Body;

public class AddIrcCredentialsFunctionBody
{
    public required string Nick { get; set; }
    public string Password { get; set; } = string.Empty;
}