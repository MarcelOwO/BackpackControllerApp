using System.Collections.ObjectModel;
using BackpackControllerApp.Enums;
using BackpackControllerApp.Models;
using BackpackControllerApp.Services.Interfaces;

namespace BackpackControllerApp.Services;

public class LoggingService : ILoggingService
{
    public ObservableCollection<LogEntry> Logs { get; } =
        [new(DateTime.Now, LogLevel.Info, "LoggingService", "Logging service started")];


    public void Log(LogLevel level, string message, string? source = null)
    {
        var entry = new LogEntry(DateTime.Now, level, source, message);
        
        Logs.Add(entry);

        if (Logs.Count > 100)
        {
            Logs.RemoveAt(0);
        }
    }
}