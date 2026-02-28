namespace ReforiaBackend.Dto.Requests.TourneyClient;

public class OpenClientRequest
{
    public string Acronym { get; set; } = string.Empty;
    public int TeamSize { get; set; }
}