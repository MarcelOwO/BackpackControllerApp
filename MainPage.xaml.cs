using BackpackControllerApp.Enums;
using BackpackControllerApp.Interfaces;
using BackpackControllerApp.Services;
using BackpackControllerApp.Views;

namespace BackpackControllerApp;

public partial class MainPage : ContentPage
{
    private readonly MediatorController _mediatorController;

    private readonly ILoggingService _loggingService;

    public MainPage(MediatorController mediatorController, ILoggingService loggingService)
    {
        InitializeComponent();

        _loggingService = loggingService;
        _mediatorController = mediatorController;

        BindingContext = new MainView(mediatorController);

        _loggingService.Log(LogLevel.Info, "MainPage initialized", "MainPage");
    }

    private void OnImageClicked(string obj)
    {
        _loggingService.Log(LogLevel.Info, "Image clicked", "MainPage");
        _mediatorController.SetDisplayed(obj);
    }

    private void SetDisplayed(string fileName)
    {
        _loggingService.Log(LogLevel.Info, "Setting displayed image", "MainPage");

        _mediatorController.SetDisplayed(fileName);
    }

    private async void Upload(object? sender, EventArgs e)
    {
        _loggingService.Log(LogLevel.Info, "Upload button clicked", "MainPage");

        await _mediatorController.AddFile();
    }
}