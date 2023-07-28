using MVCSite.Interfaces;
using MVCSite.Models;
using MVCSite.Features.Enums;

namespace MVCSite.Features.Services;

public class ImageService : IImageService
{
    private readonly string _hostEnviroment;
    public ImageService(IWebHostEnvironment hostEnvironment, IDBManager db)
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
    public async Task<(string Url, Guid Id, ImageUploadStatusCode Status)> Upload(string base64String, string area)
    {
        if (string.IsNullOrEmpty(base64String))
            throw new ArgumentException("Base64 doesn't be empty or null.", nameof(base64String));
        var base64Parts = base64String.Split(',');
        if (base64Parts.Length != 2)
            throw new ArgumentException("Incorrect format base64.", nameof(base64String));

        var contentType = base64Parts[0].Split(':')[1].Split(';')[0];

        // Конвертируем base64 строку в массив байтов
        var base64Data = Convert.FromBase64String(base64Parts[1]);

        // Создаем временный MemoryStream для сохранения данных
        using var memoryStream = new MemoryStream(base64Data);

        // Создаем временный файл на основе MemoryStream
        var file = new FormFile(memoryStream, 0, base64Data.Length, null!, "temp")
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };
        return await Upload(file, area);
    }
}