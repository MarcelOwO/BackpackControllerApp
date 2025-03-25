using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using BackpackControllerApp.Enums;
using BackpackControllerApp.Services.Interfaces;

namespace BackpackControllerApp.ViewModels;

public sealed class MainPageViewModel : INotifyPropertyChanged
{
    public ObservableCollection<string> Files => _storageService.Files;
    public ObservableCollection<string> Thumbnails => _storageService.Thumbnails;
    public string BluetoothStatus => _bluetoothService.IsConnected;
    public string SelectedImage { get; set; }

    private readonly IStorageService _storageService;
    private readonly ILoggingService _loggingService;
    private readonly IBluetoothService _bluetoothService;
    private readonly IImageService _imageService;
    
    public ICommand UploadCommand { get; private set; }

    public MainPageViewModel(ILoggingService loggingService, IBluetoothService bluetoothService,
        IImageService imageService, IStorageService storageService)
    {
        _loggingService = loggingService;
        _bluetoothService = bluetoothService;
        _imageService = imageService;
        _storageService = storageService;
        
        SelectedImage = "";

        Thumbnails.CollectionChanged += (sender, args) => OnPropertyChanged(nameof(Thumbnails));
        Files.CollectionChanged += (sender, args) => OnPropertyChanged(nameof(Files));

        UploadCommand = new Command(async void () =>
        {
            try
            {
                await AddFile();
            }
            catch (Exception e)
            {
                _loggingService.Log(LogLevel.Error, e.Message, "MainPage");
            }
        });
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private async Task AddFile()
    {
        _loggingService.Log(LogLevel.Info, "Adding file", "MediatorController");

        var file = await MediaPicker.PickPhotoAsync(new MediaPickerOptions { Title = "Select File" });

        if (file == null)
        {
            _loggingService.Log(LogLevel.Warning, "No file selected", "MainPage");
            return;
        }

        var processedFile = await _imageService.ProcessFile(file);

        processedFile.Name = Guid.NewGuid();

        await _storageService.SaveFile(processedFile);
    }
}