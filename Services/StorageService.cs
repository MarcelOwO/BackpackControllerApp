using System.Collections.ObjectModel;
using BackpackControllerApp.Enums;
using BackpackControllerApp.Models;
using BackpackControllerApp.Services.Interfaces;

namespace BackpackControllerApp.Services;

public class StorageService : IStorageService
{
    private static readonly string CommonDirectory =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "BackpackControllerApp");

    private readonly string _thumbnailDirectory = Path.Combine(CommonDirectory, "Thumbnails");
    private readonly string _uploadedDirectory = Path.Combine(CommonDirectory, "Uploaded");

    private readonly ILoggingService _loggingService;

    public ObservableCollection<string> Files { get; set; } = [];
    public ObservableCollection<ThumbnailData> Thumbnails { get; set; } = [];

    public StorageService(ILoggingService loggingService)
    {
        _loggingService = loggingService;

        if (!Directory.Exists(CommonDirectory))
        {
            Directory.CreateDirectory(CommonDirectory);
        }

        if (!Directory.Exists(_thumbnailDirectory))
        {
            Directory.CreateDirectory(_thumbnailDirectory);
        }

        if (!Directory.Exists(_uploadedDirectory))
        {
            Directory.CreateDirectory(_uploadedDirectory);
        }

        //replace late with proper settings/persistance file

        var thumbnails = Directory.GetFiles(_thumbnailDirectory);

        foreach (var file in thumbnails)
        {
            Thumbnails.Add(new ThumbnailData()
            {
                Name = Path.GetFileName(file),
                Path = file
            });
        }

        var files = Directory.GetFiles(_uploadedDirectory);

        foreach (var file in files)
        {
            Files.Add(file);
        }


        _loggingService.Log(LogLevel.Info, "StorageController initialized", "StorageController");
    }

    public async Task SaveFile(FileResult file)
    {
        _loggingService.Log(LogLevel.Info, "Saving file", "StorageController");

        await using var fileStream = new FileStream(file.FullPath, FileMode.Open);

        var newFile = new FileInfo(Path.Combine(_uploadedDirectory, file.FileName));
        var newThumbnail = new FileInfo(Path.Combine(_thumbnailDirectory, file.FileName));

        await fileStream.CopyToAsync(newFile.OpenWrite());


        Files.Add(newFile.FullName);
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
        Thumbnails.Add(new ThumbnailData()
        {
            Name = newFile.Name,
            Path = newThumbnail.FullName
        });
    }
}