namespace ReforiaBackend.Dto.Requests;

public class SetIrcCredentialsRequest
{
    public required string Nick { get; set; }
    public string Password { get; set; } = string.Empty;
}