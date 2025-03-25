using BackpackControllerApp.Models;

namespace BackpackControllerApp.Services.Interfaces;

public interface IImageService
{
    Task<ProcessedFile> ProcessFile(FileResult fileResult);
}