using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

public static class ReforiaLogger
{
    private static ILoggerFactory _loggerFactory;

    public static void Configure(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }

    private static ILogger GetLogger([CallerFilePath] string file = "")
    {
        var category = System.IO.Path.GetFileNameWithoutExtension(file);
        return _loggerFactory.CreateLogger(category);
    }

    public static void Info(string message, 
                            [CallerFilePath] string file = "")
    {
        var logger = GetLogger(file);
        logger.LogInformation($"{message} | {System.IO.Path.GetFileName(file)}");
    }

    public static void InfoMember(
        string message,
        [CallerMemberName] string member = "",
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0)
    {
        var logger = GetLogger(file); // nazwa kategorii = Program.cs
        logger.LogInformation($"{message} ({Path.GetFileName(file)} : {line})");
    }
}