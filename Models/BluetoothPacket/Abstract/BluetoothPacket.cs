using BackpackControllerApp.Enums;

namespace BackpackControllerApp.Models;

public abstract class BluetoothPacket 
{
    public required PacketType Type  { get; set; }
}

   
   