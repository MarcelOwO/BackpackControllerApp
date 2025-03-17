using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackpackControllerApp.Interfaces;
using BackpackControllerApp.Services;
using BackpackControllerApp.Views;

namespace BackpackControllerApp.Pages;

public partial class SettingsPage : ContentPage
{
    private readonly ILoggingService _loggingService;
    private readonly ISettingsService _settingsService;

    public SettingsPage(ILoggingService loggingService, ISettingsService settingsService)
    {
        InitializeComponent();

        _loggingService = loggingService;
        _settingsService = settingsService;
        BindingContext = new SettingsView(loggingService, settingsService);
    }
}