using System.Collections.ObjectModel;
using BackpackControllerApp.Models;

namespace BackpackControllerApp.Services.Interfaces;

public interface IStorageService
{
    Task SaveFile(ProcessedFile file);

    ObservableCollection<SavedFile> SavedFiles { get; set; } 
    void RemoveFile(SavedFile file);
}