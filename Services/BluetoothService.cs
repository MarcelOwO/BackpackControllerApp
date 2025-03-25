using BackpackControllerApp.Enums;
using BackpackControllerApp.Services.Interfaces;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;

namespace BackpackControllerApp.Services;

public class BluetoothService : IBluetoothService
{
    private readonly IBluetoothLE _bluetoothLe;
    private readonly IAdapter _adapter;

    private IDevice? _connectedDevice;

    private readonly ILoggingService _loggingService;

    public BluetoothService(ILoggingService loggingService)
    {
        _loggingService = loggingService;

        _bluetoothLe = CrossBluetoothLE.Current;
        _adapter = CrossBluetoothLE.Current.Adapter;
        
        _connectedDevice = null;
        
        loggingService.Log(LogLevel.Info, "BluetoothController initialized", "BluetoothController");
    }
}