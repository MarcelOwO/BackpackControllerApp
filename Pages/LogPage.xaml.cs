using BackpackControllerApp.Interfaces;
using BackpackControllerApp.Views;

namespace BackpackControllerApp.Pages;

public partial class LogPage : ContentPage
{
    public LogPage(ILoggingService loggingService)
    {
        InitializeComponent();
        BindingContext = new LogView(loggingService);
    }
}