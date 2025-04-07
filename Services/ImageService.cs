using BackpackControllerApp.Enums;
using BackpackControllerApp.Models;
using BackpackControllerApp.Services.Interfaces;
using SkiaSharp;

namespace BackpackControllerApp.Services;

public class ImageService : IImageService
{
    private readonly ILoggingService _loggingService;

    public ImageService(ILoggingService loggingService)
    {
        _loggingService = loggingService;
        _loggingService.Log(LogLevel.Info, "ImageController initialized", "ImageController");
    }

    public async Task<ProcessedFile> ProcessFile(FileResult fileResult)
    {
        if (fileResult.ContentType.Contains("image"))
        {
            _loggingService.Log(LogLevel.Info, "Processing image", "ImageController");
            const string type = "image";
            var processedImage = await ProcessImage(fileResult);
            var fileTask = processedImage.OpenReadAsync();
            var thumbnailData = CreateImageThumbnail(fileResult);
            return new ProcessedFile(type, fileTask, thumbnailData, Guid.NewGuid());
        }
        else if (fileResult.ContentType.Contains("video"))
        {
            _loggingService.Log(LogLevel.Info, "Processing video", "ImageController");
            _loggingService.Log(LogLevel.Warning, "Video processing not supported yet", "ImageController");

            var type = "video";
            var emptyTask = new Task<Stream>(() => new MemoryStream());

            return new ProcessedFile(type, emptyTask, emptyTask, Guid.NewGuid()); //not supported for now
        }

        _loggingService.Log(LogLevel.Error, "Unsupported file type", "ImageController");
        var emptyTask2 = new Task<Stream>(() => new MemoryStream());
        return new ProcessedFile("Null", emptyTask2, emptyTask2, Guid.NewGuid());
    }

    private async Task<Stream> CreateImageThumbnail(FileResult fileResult)
    {
        _loggingService.Log(LogLevel.Info, "Creating thumbnail", "ImageController");
        await using var stream = await fileResult.OpenReadAsync();
        using var image = SKBitmap.Decode(stream);
        var thumbnail = image.Resize(new SKImageInfo(100, 100), SKSamplingOptions.Default);
        using var thumbnailImage = SKImage.FromBitmap(thumbnail);
        var finalThumbnail = thumbnailImage.Encode(SKEncodedImageFormat.Jpeg, 100);
        return finalThumbnail.AsStream();
    }

    private async Task<byte[]> createVideoThumbnail(FileResult fileResult)
    {
        _loggingService.Log(LogLevel.Info, "Creating video thumbnail", "ImageController");
        return [];
    }

    private async Task<FileResult> ProcessVideo(FileResult fileResult)
    {
        _loggingService.Log(LogLevel.Info, "Processing video", "ImageController");
        return fileResult;
    }

    private async Task<FileResult> ProcessImage(FileResult fileResult)
    {
        _loggingService.Log(LogLevel.Info, "Processing image", "ImageController");
        return fileResult;
    }
}