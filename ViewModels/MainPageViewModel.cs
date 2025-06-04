using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using BackpackControllerApp.Enums;
using BackpackControllerApp.Models;
using BackpackControllerApp.Services.Interfaces;

namespace BackpackControllerApp.ViewModels;

public sealed class MainPageViewModel : INotifyPropertyChanged
{
    public ObservableCollection<SavedFile> Files => _storageService.SavedFiles;

    public string BluetoothStatus => _bluetoothService.IsConnected;
    public string SelectedImage { get; set; }
    public double UploadProgress { get; set; }

    private readonly IStorageService _storageService;
    private readonly ILoggingService _loggingService;
    private readonly IBluetoothService _bluetoothService;
    private readonly IImageService _imageService;

    private CancellationTokenSource? _cancellationTokenSource;

    public ICommand CancelUpload { get; private set; }
    public ICommand AddFileCommand { get; private set; }
    public ICommand SetSelectedImageCommand { get; private set; }
    public ICommand RemoveImageCommand { get; private set; }
    public ICommand UploadImageCommand { get; private set; }

    public MainPageViewModel(ILoggingService loggingService, IBluetoothService bluetoothService,
        IImageService imageService, IStorageService storageService)
    {
        _loggingService = loggingService;
        _bluetoothService = bluetoothService;
        _imageService = imageService;
        _storageService = storageService;

        SelectedImage = "";

        Files.CollectionChanged += (sender, args) => OnPropertyChanged(nameof(Files));

        AddFileCommand = new Command(async void () =>
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

        CancelUpload = new Command(async () =>
        {
            if (_cancellationTokenSource != null)
                await _cancellationTokenSource.CancelAsync();
        });

        UploadImageCommand = new Command<SavedFile>(async void (savedFile) =>
        {
            await _bluetoothService.SendFile(new FileBluetoothPacket()
                {
                    FileName = savedFile.Name,
                    Data = (await File.ReadAllBytesAsync(savedFile.OriginalPath)).ToList(),
                    Type = PacketType.File
                }, _cancellationTokenSource?.Token
                , new Progress<double>(value =>
                {
                    UploadProgress = value;
                    OnPropertyChanged(nameof(UploadProgress));
                }));
        });

        SetSelectedImageCommand = new Command<SavedFile>(async file =>
        {
            if (file == null)
            {
                _loggingService.Log(LogLevel.Warning, "No image selected", "MainPage");
                return;
            }

            SelectedImage = file.ThumbnailPath;
            OnPropertyChanged(nameof(SelectedImage));

            await _bluetoothService.SendCommand(new CommandBluetoothPacket
            {
                Command = $"Select:{file.Name}",
                Type = PacketType.Command,
            });
        });

        RemoveImageCommand = new Command<SavedFile>(file =>
        {
            if (file == null)
            {
                _loggingService.Log(LogLevel.Warning, "No image selected", "MainPage");
                return;
            }

            storageService.RemoveFile(file);

            OnPropertyChanged(nameof(Files));

            _bluetoothService.SendCommand(new CommandBluetoothPacket()
            {
                Command = $"Delete:{file.Name}",
                Type = PacketType.Command
            });
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

        await _storageService.SaveFile(processedFile);
    }
}