using MVCSite.Interfaces;
using MVCSite.Models;
using MVCSite.Features.Enums;

namespace MVCSite.Features.Services;

public class ImageService : IImageService
{
    private readonly string _hostEnviroment;
    public ImageService(IWebHostEnvironment hostEnvironment)
    {
        _hostEnviroment = hostEnvironment.ContentRootPath;
    }
    public async Task<(string Url, Guid Id, ImageUploadStatusCode Status)> Upload(IFormFile uploadedFile, string area)
    {
        if(uploadedFile == null)
            return (null!, Guid.Empty, ImageUploadStatusCode.Error);
        var id = Guid.NewGuid();
        var imagePath = _hostEnviroment + $"/AppData/{area}/" + id.ToString() + ".jpg";
        using (var fileStream = new FileStream(imagePath, FileMode.Create))
        {
            await uploadedFile.CopyToAsync(fileStream);
        }
        return ("/api/Image/Show?" + "id=" + id.ToString() + '&' + "imageArea=" + area, id, ImageUploadStatusCode.Success);
    }
}