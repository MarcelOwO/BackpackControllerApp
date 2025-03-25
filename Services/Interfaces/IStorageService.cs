using System.Collections.ObjectModel;
using BackpackControllerApp.Models;

namespace BackpackControllerApp.Services.Interfaces;

public interface IStorageService
{
    Task SaveFile(ProcessedFile file);
    
    ObservableCollection<string> Files { get; set; }
    
    ObservableCollection<string> Thumbnails { get; set; }
}