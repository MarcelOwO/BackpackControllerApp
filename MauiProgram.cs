using BackpackControllerApp.Interfaces;
using BackpackControllerApp.Services;
using Microsoft.Extensions.Logging;
using IImageController = BackpackControllerApp.Interfaces.IImageController;

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
        
        builder.Services.AddSingleton<MediatorController>();
        
        builder.Services.AddSingleton<IBluetoothController,BluetoothController>();
        builder.Services.AddSingleton<IStorageController,StorageController>();
        builder.Services.AddSingleton<IImageController,ImageController>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
