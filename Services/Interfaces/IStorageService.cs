using System.Collections.ObjectModel;
using BackpackControllerApp.Models;

namespace BackpackControllerApp.Services.Interfaces;

public interface IStorageService
{
    Task SaveFile(ProcessedFile file);
    Task SaveFile(FileResult file);
    ObservableCollection<string> Files { get; set; }
    
    ObservableCollection<ThumbnailData> Thumbnails { get; set; }
}