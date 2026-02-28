namespace ReforiaBackend.Dto.Requests;

public class ChangeSettingsRequest
{
    public string ApiToken { get; set; } = string.Empty;
    public bool IsUserHighlighted { get; set; }
    public string Language { get; set; } = string.Empty;
    
    public bool AlertOnMention { get; set; }
    
    public bool AlertOnKeyword { get; set; }
    
    public bool HighlightOnKeyword { get; set; }
    
    public List<string> KeywordList { get; set; } = new List<string>();
    
    public bool ShowBeatmapBanner { get; set; }
    
    public string DefaultStartValue { get; set; } = string.Empty;
    
    public string DefaultTimerValue { get; set; } = string.Empty;
    
    public string OsuTourneyClientPathFolder { get; set; } = string.Empty;
}