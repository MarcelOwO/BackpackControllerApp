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


    public ObservableCollection<SavedFile> SavedFiles { get; set; } = [];

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


        var thumbnails = Directory.GetFiles(_thumbnailDirectory);
        var files = Directory.GetFiles(_uploadedDirectory);

        foreach (var thumbnail in thumbnails)
        {
            var thumbnailName = Path.GetFileName(thumbnail);

            foreach (var fullFile in files)
            {
                var fullFileName = Path.GetFileName(fullFile);

                if (thumbnailName != fullFileName)
                    continue;
                SavedFiles.Add(new SavedFile()
                {
                    Name = fullFileName,
                    OriginalPath = fullFile,
                    ThumbnailPath = thumbnail
                });
            }
        }

        _loggingService.Log(LogLevel.Info, "StorageController initialized", "StorageController");
    }

    public void RemoveFile(SavedFile fileToRemove)
    {
        try
        {
            var copy = fileToRemove;
            if (!SavedFiles.Remove(fileToRemove))
            {
                _loggingService.Log(LogLevel.Error, "File not found in saved files", "StorageController");
                return;
            }

            File.Delete(fileToRemove.OriginalPath);
            File.Delete(fileToRemove.ThumbnailPath);
        }
        catch (Exception e)
        {
            _loggingService.Log(LogLevel.Error, e.Message, "StorageController");
        }
    }


    public async Task SaveFile(ProcessedFile file)
    {
        _loggingService.Log(LogLevel.Info, "Saving file", "StorageController");
        var fileName = file.Name;

        using var fileStream = file.FileTask;
        using var thumbnailStream = file.ThumbnailTask;

        var newFile = new FileInfo(Path.Combine(_uploadedDirectory, fileName));
        var newThumbnail = new FileInfo(Path.Combine(_thumbnailDirectory, fileName));

        var saveFileTask = (await fileStream).CopyToAsync(newFile.OpenWrite());
        var saveThumbnailTask = (await thumbnailStream).CopyToAsync(newThumbnail.OpenWrite());

        await Task.WhenAll(saveFileTask, saveThumbnailTask);

        SavedFiles.Add(new SavedFile()
        {
            Name = fileName,
            OriginalPath = newFile.FullName,
            ThumbnailPath = newThumbnail.FullName,
        });
    }
}