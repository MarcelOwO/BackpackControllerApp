using BackpackControllerApp.Services.Interfaces;
using BackpackControllerApp.ViewModels;

namespace BackpackControllerApp.Views;

public partial class SettingsPage 
{
    public SettingsPage(ILoggingService loggingService, ISettingsService settingsService)
    {
        InitializeComponent();

        BindingContext = new SettingsPageViewModel(loggingService, settingsService);
    }
}