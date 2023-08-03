using MVCSite.Features.Enums;

namespace MVCSite.Interfaces;
public interface IImageService
{
    public Task<(string Url, Guid Id, ImageUploadStatusCode Status)> Upload(IFormFile uploadedFile, string area);
    public Task<bool> Delete(Guid id, string area);
    public string GetLink(Guid id, string area);
}