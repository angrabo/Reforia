using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Reforia.Core.Utils;

public static class Logger
{
    private static ILoggerFactory? _factory;

    public static void Init(ILoggerFactory factory) => _factory = factory;

    public static void Info(string message) => Log(LogLevel.Information, message);
    
    public static void Debug(string message) => Log(LogLevel.Debug, message);
    
    public static void Warning(string message) => Log(LogLevel.Warning, message);
    
    public static void Warning(Exception e, string message) => Log(LogLevel.Warning, message, e);
    
    public static void Error(string message) => Log(LogLevel.Error, message);

    public static void Error(Exception ex, string message) => Log(LogLevel.Error, message, ex);
    
    public static void Fatal(string message) => Log(LogLevel.Critical, message);
    
    public static void Fatal(Exception ex, string message) => Log(LogLevel.Critical, message, ex);
    
    private static void Log(LogLevel level, string message, Exception? ex = null)
    {
        if (_factory == null) return;

        var frame = new StackFrame(2, false);
        var method = frame.GetMethod();
        var type = method?.DeclaringType;

        if (type == null) return;

        if (type.Name.Contains("<") && type.DeclaringType != null) 
            type = type.DeclaringType;

        var categoryName = type.FullName ?? "Global";

        var logger = _factory.CreateLogger(categoryName);
        logger.Log(level, ex, message);
    }
}