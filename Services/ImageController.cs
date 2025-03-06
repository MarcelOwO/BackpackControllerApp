using Android.Provider;
using BackpackControllerApp.Models;

namespace BackpackControllerApp.Services;

public class ImageController
{
    private readonly MediatorController _mediatorController;

    public ImageController(MediatorController mediatorController)
    {
        _mediatorController = mediatorController;

        _mediatorController.OnFileResult += async (file) => { await processFile(file); };
    }

    public async Task processFile(FileResult fileResult)
    {
        if (fileResult.ContentType.Contains("image"))
        {
            await processImage(fileResult);
            await createThumbnail(fileResult);
        }

        if (fileResult.ContentType.Contains("video"))
        {
            await processVideo(fileResult);
            await createThumbnail(fileResult);
        }
    }
    
    private async Task createThumbnail(FileResult fileResult)
    {
        var res = BackpackControllerApp.Configuration.Settings.Instance.ScreenResolution;
       //process later 
        _mediatorController.OnThumbnailCreated(fileResult);
    }

    private async Task processVideo(FileResult fileResult)
    {
        var res = BackpackControllerApp.Configuration.Settings.Instance.ScreenResolution;
        //process later
        _mediatorController.OnProcessedFile(fileResult);
        
    }

    private async Task processImage(FileResult fileResult)
    {
         var res = BackpackControllerApp.Configuration.Settings.Instance.ScreenResolution; 
         //process later
         _mediatorController.OnProcessedFile(fileResult);
         
    }
}