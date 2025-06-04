using BackpackControllerApp.Enums;

namespace BackpackControllerApp.Models;

public class FileBluetoothPacket :BluetoothPacket
{
    public required string FileName { get; set; }
    public required List<byte> Data { get; set; }
}