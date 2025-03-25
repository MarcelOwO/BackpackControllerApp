using System.Collections.ObjectModel;
using BackpackControllerApp.Models;
using BackpackControllerApp.Services.Interfaces;

namespace BackpackControllerApp.ViewModels;

public class LogPageViewModel(ILoggingService loggingService)
{
    private readonly ILoggingService _loggingService = loggingService;
    
    public ObservableCollection<LogEntry> Logs => _loggingService.Logs;
}