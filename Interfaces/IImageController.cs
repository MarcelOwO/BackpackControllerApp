using BackpackControllerApp.Models;

namespace BackpackControllerApp.Interfaces;

public interface IImageController
{
    Task<ProcessedFile> ProcessFile(FileResult fileResult);
}