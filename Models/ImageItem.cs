namespace BackpackControllerApp.Models;

public class ImageItem(string id, string fileName, string filePath, string thumbnailPath)
{
    private string Id { get; set; } = id;
    private string FileName { get; set; } = fileName;
    private string FilePath { get; set; } = filePath;
    private string ThumbnailPath { get; set; } = thumbnailPath;
}