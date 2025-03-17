using System.Collections.ObjectModel;
using BackpackControllerApp.Models;

namespace BackpackControllerApp.Interfaces;

public interface IStorageController
{
    Task SaveFile(ProcessedFile file);
    
    ObservableCollection<string> Files { get; set; }
    
    ObservableCollection<string> Thumbnails { get; set; }
}