using Android.Graphics;
using BackpackControllerApp.Services;
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

        builder.Services.AddSingleton<MediatorController>();

        builder.Services.AddTransient<BluetoothController>();
        builder.Services.AddTransient<StorageController>();
        builder.Services.AddTransient<ImageController>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}