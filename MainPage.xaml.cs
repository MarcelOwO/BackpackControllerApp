using BackpackControllerApp.Models;
using BackpackControllerApp.Services;

namespace BackpackControllerApp;

public partial class MainPage : ContentPage
{
    private string _selectedImages = "";
    private readonly MediatorController _mediatorController;

    public string BluetoothStatus { get; set; } = "";
    public List<ImageItem> UploadedFiles { get; set; }


    public MainPage(MediatorController mediatorController)
    {
        InitializeComponent();

        _mediatorController = mediatorController;

        UploadedFiles = [];

        _mediatorController.OnConnectionChanged += status => BluetoothStatus = status;
        _mediatorController.OnDisplayedChanged += file => _selectedImages = file;

        Task.Run(async () => await UpdateLocalData());
    }

    private async Task TryConnect()
    {
        await _mediatorController.TryConnect();
    }

    private void OnImageClicked(string obj)
    {
        _mediatorController.SetDisplayed(obj);
    }

    private async Task UploadFile(string fileName)
    {
#pragma warning disable CA1416
        var file = await MediaPicker.PickPhotoAsync(new MediaPickerOptions { Title = "Select File" });
#pragma warning restore CA1416

        if (file == null) return;

        _mediatorController.AddFile(file);

        return;
    }

    private void SetDisplayed(string fileName)
    {
        _mediatorController.SetDisplayed(fileName);
    }

    private async Task UpdateLocalData()
    {
        _mediatorController.waitForConnection();
        _selectedImages = await _mediatorController.GetDisplayed();
        UploadedFiles = await _mediatorController.GetUploaded();
    }

    private void UploadFile(object? sender, EventArgs e)
    {
        Task.Run(() => UploadFile(_selectedImages));
    }

    private void Button_OnClicked(object? sender, EventArgs e)
    {
        SetDisplayed(_selectedImages);
    }

    private void TryConnect(object? sender, TappedEventArgs e)
    {
        Task.Run(async () => await TryConnect());
    }
}