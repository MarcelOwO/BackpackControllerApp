using BackpackControllerApp.Models;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;

namespace BackpackControllerApp.Services;

public class MediatorController
{
    public MediatorController()
    {
    }

    public event Action<string>? OnDisplayedChanged;

    public void SetDisplayed(string fileName)
    {
        OnDisplayedChanged?.Invoke(fileName);
    }

    public event Action<string>? OnConnectionChanged;

    public void waitForConnection()
    {
        throw new NotImplementedException();
    }

    public async Task<string> GetDisplayed()
    {
        throw new NotImplementedException();
    }

    public async Task<List<ImageItem>> GetUploaded()
    {
        throw new NotImplementedException();
    }

    public async Task TryConnect()
    {
        throw new NotImplementedException();
    }

    public void BluetoothConnectionChanged(object? sender, BluetoothStateChangedArgs e)
    {
#pragma warning disable CA1416
        OnConnectionChanged?.Invoke(e.NewState.ToString());
#pragma warning restore CA1416
    }

    public void SetState(BluetoothState bluetoothLeState)
    {
        OnConnectionChanged?.Invoke(bluetoothLeState.ToString());
    }

    public event Action<FileResult>? OnFileResult;

    public void AddFile(FileResult fileResult)
    {
#pragma warning disable CA1416
        OnFileResult?.Invoke(fileResult);
#pragma warning restore CA1416
    }

    public event Action<FileResult>? OnFileProcessed;

    public void OnProcessedFile(FileResult fileResult)
    {
#pragma warning disable CA1416
        OnFileProcessed?.Invoke(fileResult);
#pragma warning restore CA1416
    }

    public event Action<FileResult>? OnThumbnailProcessed;

    public void OnThumbnailCreated(FileResult fileResult)
    {
#pragma warning disable CA1416
        OnThumbnailProcessed?.Invoke(fileResult);
#pragma warning restore CA1416
    }

    public event Action<List<string>>? OnFilesUpdated;

    public void OnFileSaved(List<string> files)
    {
        OnFilesUpdated?.Invoke(files.ToList());
    }

    public event Action<List<string>>? OnThumbnailsUpdated;

    public void OnThumbnailSaved(List<string> thumnails)
    {
        OnThumbnailsUpdated?.Invoke(thumnails.ToList());
    }
}