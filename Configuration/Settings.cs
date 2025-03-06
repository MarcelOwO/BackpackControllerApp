namespace BackpackControllerApp.Configuration;

public class Settings
{
    private static readonly Lazy<Settings> _instance = new(() => new Settings());
    public static Settings Instance => _instance.Value;

    private Settings()
    {
    }

    public (int x, int y) ScreenResolution { get; set; } = (1920, 1080);
    public string ConnectionKey = "SuperSecureKey"; //need to think of something better
}