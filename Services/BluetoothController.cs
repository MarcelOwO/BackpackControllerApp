using BackpackControllerApp.Enums;
using BackpackControllerApp.Interfaces;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;

namespace BackpackControllerApp.Services;

public class BluetoothController : IBluetoothController
{
    public string IsConnected { get; set; }
    private readonly IBluetoothLE _bluetoothLe;
    private readonly IAdapter _adapter;
    private IDevice _connectedDevice;
    private readonly ILoggingService _loggingService;

    public event Action<string>? OnConnectionChanged;

    public BluetoothController(ILoggingService loggingService)
    {
        _loggingService = loggingService;

        _bluetoothLe = CrossBluetoothLE.Current;
        _adapter = CrossBluetoothLE.Current.Adapter;

        _bluetoothLe.StateChanged += (state, e) => IsConnected = e.NewState.ToString();

        loggingService.Log(LogLevel.Info, "BluetoothController initialized", "BluetoothController");
    }
    
    
    
    
    
}