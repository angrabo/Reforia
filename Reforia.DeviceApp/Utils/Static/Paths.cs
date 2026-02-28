namespace ReforiaBackend.Utils.Static;

public static class Paths
{
    private static readonly string BasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Reforia");
    
    public static string LogFolderPath => Path.Combine(BasePath, "logs");
    public static string LogFilePath => Path.Combine(LogFolderPath, "reforia.log");
    
    public static string DatabasePath => Path.Combine(BasePath, "reforia.db");
    
}