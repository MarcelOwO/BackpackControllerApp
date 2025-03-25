using System.ComponentModel;
using System.Runtime.CompilerServices;
using BackpackControllerApp.Services.Interfaces;

namespace BackpackControllerApp.ViewModels;

public sealed class SettingsPageViewModel : INotifyPropertyChanged
{
    private readonly ISettingsService _settingsService;
    private readonly ILoggingService _loggingService;
    
    public SettingsPageViewModel(ILoggingService loggingService, ISettingsService settingsService)
    {
        _settingsService = settingsService;
        _loggingService = loggingService;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}