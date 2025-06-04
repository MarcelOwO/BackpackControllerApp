using Java.Util;

namespace BackpackControllerApp.Services.Interfaces;

public interface ISettingsService
{
    public string TargetAddress { get; set; }
    public UUID Uuid { get; set; }
}