namespace BackpackControllerApp.Models;

public class ProcessedFile(string type, Task<Stream> fileTask, Task<Stream> thumbnailTask, Guid name)
{
    public string Type { get; set; } = type;
    public Task<Stream> FileTask { get; set; } = fileTask;
    public Task<Stream> ThumbnailTask { get; set; } = thumbnailTask;
    public Guid Name { get; set; } = name;
}