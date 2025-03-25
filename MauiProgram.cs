using BackpackControllerApp.Services;
using BackpackControllerApp.Services.Interfaces;
using BackpackControllerApp.ViewModels;
using BackpackControllerApp.Views;
using Microsoft.Extensions.Logging;

namespace BackpackControllerApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton<ILoggingService, LoggingService>();
        builder.Services.AddSingleton<ISettingsService, SettingsService>();

        builder.Services.AddSingleton<IBluetoothService, BluetoothService>();
        builder.Services.AddSingleton<IStorageService, StorageService>();
        builder.Services.AddSingleton<IImageService, ImageService>();

        builder.Services.AddTransient<MainPageViewModel>();
        builder.Services.AddTransient<LogPageViewModel>();
        builder.Services.AddTransient<SettingsPageViewModel>();

        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<LogPage>();
        builder.Services.AddTransient<SettingsPage>();
        
#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}