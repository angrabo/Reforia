namespace ReforiaBackend.Dto.Requests.App;

public class SaveLogRequest
{
    public string Message { get; set; } = string.Empty;
    public string Level { get; set; } = "Info"; // "Trace", "Debug", "Info", "Warning", "Error", "Critical"
}