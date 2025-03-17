namespace BackpackControllerApp.Models;

public class ProcessedFile
{
    public string Type { get; set; }
    public Task<Stream> FileTask { get; set; }
    
    public Task<Stream> ThumbnailTask { get; set; }
    
    public Guid Name { get; set; }
}