using BackpackControllerApp.Services.Interfaces;
using Java.Util;

namespace BackpackControllerApp.Services;

public class SettingsService : ISettingsService
{
    public string TargetAddress { get; set; }
   //public UUID Uuid { get; set; } = UUID.FromString("00001101-0000-1000-8000-00805f9b34fb");
    public UUID Uuid { get; set; } = UUID.FromString("60696969-6969-6969-6969-696969696969");
}