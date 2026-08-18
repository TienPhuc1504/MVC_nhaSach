namespace MVC_nhaSach.Services;

public interface IImageService
{
    Task<string> SaveBookImageAsync(IFormFile image, CancellationToken cancellationToken = default);
    void DeleteBookImage(string? relativePath);
}
