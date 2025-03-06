namespace BackpackControllerApp.Services;

public class StorageController
{
    private static readonly string CommonDirectory =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "BackpackControllerApp");

    private readonly string _thumbnailDirectory = Path.Combine(CommonDirectory, "Thumbnails");
    private readonly string _uploadedDirectory = Path.Combine(CommonDirectory, "Uploaded");

    private readonly MediatorController _mediatorController;

    public StorageController(MediatorController mediatorController)
    {
        _mediatorController = mediatorController;
        _mediatorController.OnFileProcessed += async (file) =>
        {
            await SaveFile(file, _uploadedDirectory);

            var files = Directory.GetFiles(_uploadedDirectory).Select(x => Path.Combine(_uploadedDirectory, x))
                .ToList();
            _mediatorController.OnFileSaved(files);
        };
        _mediatorController.OnThumbnailProcessed += async (file) =>
        {
            await SaveFile(file, _thumbnailDirectory);
            
            var files = Directory.GetFiles(_thumbnailDirectory).Select(x => Path.Combine(_uploadedDirectory, x))
                .ToList();
            _mediatorController.OnThumbnailSaved(files);
        };
    }

    private async Task SaveFile(FileResult file, string location)
    {
        await using var stream = await file.OpenReadAsync();
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        var bytes = memoryStream.ToArray();
        var filePath = Path.Combine(location, file.FileName);
        await File.WriteAllBytesAsync(filePath, bytes);
    }
}