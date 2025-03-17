using BackpackControllerApp.Interfaces;

namespace BackpackControllerApp.Views;

public class SettingsView
{
    
    private readonly ISettingsService _settingsService;
    private readonly ILoggingService _loggingService;
    
    public SettingsView(ILoggingService loggingService, ISettingsService settingsService)
    {
        _settingsService = settingsService;
        _loggingService = loggingService;
    }
}