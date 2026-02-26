namespace Reforia.Core.Modules.Communication.Functions.Body;

public class CreateIrcConnectionFunctionBody
{
    public string ConnectionId { get; set; } = string.Empty;
    public required string Host { get; set; }
    public required string Nick { get; set; }
    public string Password { get; set; } = string.Empty;
    
    public int Port { get; set; } = 6667;
}