using System.Collections.ObjectModel;
using BackpackControllerApp.Enums;
using BackpackControllerApp.Models;

namespace BackpackControllerApp.Services.Interfaces;

public interface ILoggingService
{
    public ObservableCollection<LogEntry> Logs { get; }
    public void Log(LogLevel level, string message, string? source = null);
}