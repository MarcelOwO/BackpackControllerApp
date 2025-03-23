using System.Collections.ObjectModel;
using BackpackControllerApp.Enums;
using BackpackControllerApp.Interfaces;
using IImageController = BackpackControllerApp.Interfaces.IImageController;

namespace BackpackControllerApp.Services;

public class MediatorController
{
    private readonly ILoggingService _loggingService;
    private readonly IBluetoothController _bluetoothController;
    private readonly IStorageController _storageController;
    private readonly IImageController _imageController;

    public ObservableCollection<string> Files => _storageController.Files;
    public ObservableCollection<string> Thumbnails => _storageController.Thumbnails;

    public MediatorController(ILoggingService loggingService, IBluetoothController bluetoothController,
        IStorageController storageController, IImageController imageController)
    {
        _loggingService = loggingService;

        _bluetoothController = bluetoothController;
        _storageController = storageController;
        _imageController = imageController;

        _loggingService.Log(LogLevel.Info, "MediatorController initialized", "MediatorController");
    }

    public string BluetoothStatus => _bluetoothController.IsConnected;
    public string SelectedImage { get; set; }

    public void SetDisplayed(string fileName)
    {
        _loggingService.Log(LogLevel.Info, "Setting displayed image", "MediatorController");
    }

    public async Task TryConnect()
    {
        _loggingService.Log(LogLevel.Info, "Trying to connect", "MediatorController");
    }

    public async Task AddFile()
    {
        _loggingService.Log(LogLevel.Info, "Adding file", "MediatorController");

        var file = await MediaPicker.PickPhotoAsync(new MediaPickerOptions { Title = "Select File" });

        if (file == null)
        {
            _loggingService.Log(LogLevel.Warning, "No file selected", "MainPage");
            return;
        }

        var processedFile = await _imageController.ProcessFile(file);

        processedFile.Name = Guid.NewGuid();

        await _storageController.SaveFile(processedFile);
    }
}