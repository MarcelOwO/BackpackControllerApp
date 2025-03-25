using BackpackControllerApp.Services.Interfaces;
using BackpackControllerApp.ViewModels;

namespace BackpackControllerApp.Views;

public partial class LogPage 
{
    public LogPage(ILoggingService loggingService)
    {
        InitializeComponent();
        BindingContext = new LogPageViewModel(loggingService);
    }
}