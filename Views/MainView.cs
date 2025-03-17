using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BackpackControllerApp.Services;

namespace BackpackControllerApp.Views;

public class MainView : INotifyPropertyChanged
{
    private readonly MediatorController _mediatorController;
    public string SelectedImage => _mediatorController.BluetoothStatus;
    public string BluetoothStatus => _mediatorController.BluetoothStatus;
    public ObservableCollection<string> Thumbnails => _mediatorController.Thumbnails;
    public ObservableCollection<string> Files => _mediatorController.Files;

    
    public MainView(MediatorController mediatorController)
    {
        _mediatorController = mediatorController;
        Thumbnails.CollectionChanged += (sender, args) => OnPropertyChanged(nameof(Thumbnails));
        Files.CollectionChanged += (sender, args) => OnPropertyChanged(nameof(Files));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}