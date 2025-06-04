using BackpackControllerApp.Models;

namespace BackpackControllerApp.Services.Interfaces;

public interface IBluetoothService
{
    public string IsConnected { get; set; }
    public Task SendFile(FileBluetoothPacket packet, CancellationToken? token=null, IProgress<double>? progress = null);
    public Task SendCommand(CommandBluetoothPacket packet);
}