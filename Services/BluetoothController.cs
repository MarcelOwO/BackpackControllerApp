using BackpackControllerApp.Enums;
using BackpackControllerApp.Interfaces;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;

namespace BackpackControllerApp.Services;

public class BluetoothController : IBluetoothController
{
    private readonly IBluetoothLE _bluetoothLe;
    private readonly IAdapter _adapter;
    
    private IDevice _connectedDevice;
    
    private readonly ILoggingService _loggingService;
private readonly MediatorController _mediatorController;

    public BluetoothController(ILoggingService loggingService, MediatorController mediatorController)
    {
        _loggingService = loggingService;
        _mediatorController = mediatorController;
        
        
        _bluetoothLe = CrossBluetoothLE.Current;
        _adapter = CrossBluetoothLE.Current.Adapter;

        loggingService.Log(LogLevel.Info, "BluetoothController initialized", "BluetoothController");
    }
    
    
    
    
    
}