using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;

namespace BackpackControllerApp.Services;

public class BluetoothController
{
    public bool IsConnected { get; set; }

    private readonly IBluetoothLE _bluetoothLe;
    private readonly IAdapter _adapter;
    private IDevice _connectedDevice;
    
    private readonly MediatorController _mediatorController;

    public BluetoothController(MediatorController mediatorController)
    {
        _mediatorController = mediatorController;
        
        _bluetoothLe = CrossBluetoothLE.Current;
        _adapter = CrossBluetoothLE.Current.Adapter;
        
        _mediatorController.SetState(_bluetoothLe.State);
        _bluetoothLe.StateChanged += _mediatorController.BluetoothConnectionChanged;
        
    }

    public async Task ConnectToDevice(IDevice device)
    {
        await _adapter.ConnectToDeviceAsync(device);
        _connectedDevice = device;
    }
    
    
    
    

    public void SetDisplayed()
    {
    }

    public async Task UploadFile(string filePath)
    {
        if (_connectedDevice == null)
        {
            return;
        }

        var service = await _connectedDevice.GetServiceAsync(Guid.Parse("Test"));

        if (service == null)
        {
            return;
        }
        
        var characteristic = await service.GetCharacteristicAsync(Guid.Parse("Test"));

        if (characteristic == null)
        {
            return;
        }
        var fileData = File.ReadAllBytes(filePath);
        
        var chunkSize = 20;
        for (int i = 0; i < fileData.Length; i += chunkSize)
        {
            var chunk = fileData.Skip(i).Take(chunkSize).ToArray();
            await characteristic.WriteAsync(chunk);
        }

    }
}