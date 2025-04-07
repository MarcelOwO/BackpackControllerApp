namespace BackpackControllerApp.Models;

public class CommandBluetoothPacket :BluetoothPacket
{
    public required string Command { get; set; }
}