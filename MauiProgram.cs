using Android;
using Android.Content.PM;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using BackpackControllerApp.Services;
using BackpackControllerApp.Services.Interfaces;
using BackpackControllerApp.ViewModels;
using BackpackControllerApp.Views;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;

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
            })
            .ConfigureLifecycleEvents(lifecycle =>
            {
#if ANDROID
                lifecycle.AddAndroid(android =>
                {
                    android.OnCreate((activity, bundle) =>
                    {
                        if (ContextCompat.CheckSelfPermission(activity, Manifest.Permission.BluetoothConnect) !=
                            (int)Permission.Granted)
                        {
                            ActivityCompat.RequestPermissions(activity,
                                [Manifest.Permission.BluetoothConnect], 0);
                        }

                        var action = activity.Intent?.Action;
                        var data = activity.Intent?.Data?.ToString();

                        if (action == Android.Content.Intent.ActionView && data is not null)
                        {
                            Console.WriteLine($"View: {data}");
                        }
                    });
                });
#endif
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