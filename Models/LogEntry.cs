using BackpackControllerApp.Enums;

namespace BackpackControllerApp.Models;

public class LogEntry(DateTime time, LogLevel level, string? source, string logMessage)
{
    public DateTime Timestamp { get; set; } = time;
    public LogLevel Level { get; set; } = level;
    public string? Source { get; set; } = source;
    public string Message { get; set; } = logMessage;
}