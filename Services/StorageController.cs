using System.Collections.ObjectModel;
using BackpackControllerApp.Enums;
using BackpackControllerApp.Interfaces;
using BackpackControllerApp.Models;

namespace BackpackControllerApp.Services;

public class StorageController : IStorageController
{
    private static readonly string CommonDirectory =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "BackpackControllerApp");

    private readonly string _thumbnailDirectory = Path.Combine(CommonDirectory, "Thumbnails");
    private readonly string _uploadedDirectory = Path.Combine(CommonDirectory, "Uploaded");

    private readonly ILoggingService _loggingService;

    public ObservableCollection<string> Files { get; set; } = [];
    public ObservableCollection<string> Thumbnails { get; set; } = [];

    public StorageController(ILoggingService loggingService)
    {
        _loggingService = loggingService;
        _loggingService.Log(LogLevel.Info, "StorageController initialized", "StorageController");
    }

    public async Task SaveFile(ProcessedFile file)
    {
        _loggingService.Log(LogLevel.Info, "Saving file", "StorageController");

        using var fileStream = file.FileTask;
        using var thumbnailStream = file.ThumbnailTask;

        var newFile = new FileInfo(Path.Combine(_uploadedDirectory, file.Name.ToString()));
        var newThumbnail = new FileInfo(Path.Combine(_thumbnailDirectory, file.Name.ToString()));

        var a = (await fileStream).CopyToAsync(newFile.OpenWrite());
        var b = (await thumbnailStream).CopyToAsync(newThumbnail.OpenWrite());

        await Task.WhenAll(a, b);

        Files.Add(newFile.FullName);
        Thumbnails.Add(newThumbnail.FullName);
    }
}