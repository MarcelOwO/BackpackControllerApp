using BackpackControllerApp.Enums;
using BackpackControllerApp.Interfaces;
using BackpackControllerApp.Models;
using SkiaSharp;
using IImageController = BackpackControllerApp.Interfaces.IImageController;

namespace BackpackControllerApp.Services;

public class ImageController : IImageController
{
    private readonly ILoggingService _loggingService;

    public ImageController(ILoggingService loggingService)
    {
        _loggingService = loggingService;

        _loggingService.Log(LogLevel.Info, "ImageController initialized", "ImageController");
    }

    private async Task<Stream> createImageThumbnail(FileResult fileResult)
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

    private async Task<FileResult> processVideo(FileResult fileResult)
    {
        _loggingService.Log(LogLevel.Info, "Processing video", "ImageController");
        return fileResult;
    }

    private async Task<FileResult> processImage(FileResult fileResult)
    {
        _loggingService.Log(LogLevel.Info, "Processing image", "ImageController");
        return fileResult;
    }

    public async Task<ProcessedFile> ProcessFile(FileResult fileResult)
    {
        var temp = new ProcessedFile();

        if (fileResult.ContentType.Contains("image"))
        {
            _loggingService.Log(LogLevel.Info, "Processing image", "ImageController");
            temp.Type = "image";
            var processedImage = await processImage(fileResult);
            temp.FileTask = processedImage.OpenReadAsync();
            var thumbnailData = createImageThumbnail(fileResult);
            temp.ThumbnailTask = thumbnailData;
            return temp;
        }
        else if (fileResult.ContentType.Contains("video"))
        {
            temp.Type = "video";
            _loggingService.Log(LogLevel.Info, "Processing video", "ImageController");
            _loggingService.Log(LogLevel.Warning, "Video processing not supported yet", "ImageController");
            return temp; //not supported for now
        }

        _loggingService.Log(LogLevel.Error, "Unsupported file type", "ImageController");

        return temp;
    }
}