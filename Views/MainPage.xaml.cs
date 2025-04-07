using BackpackControllerApp.Services.Interfaces;
using BackpackControllerApp.ViewModels;

namespace BackpackControllerApp.Views;

public partial class MainPage
{
    private readonly ISettingsService _settingsService;

    public MainPage(ILoggingService loggingService, ISettingsService settingsService,
        IBluetoothService bluetoothService, IImageService imageService, IStorageService storageService)
    {
        _settingsService = settingsService;

        InitializeComponent();
        BindingContext = new MainPageViewModel(loggingService, bluetoothService, imageService, storageService);
    }
}