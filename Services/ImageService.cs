namespace MVC_nhaSach.Services;

public class ImageService(IWebHostEnvironment environment) : IImageService
{
    public const long MaxFileSize = 2 * 1024 * 1024;
    private static readonly HashSet<string> AllowedExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png" };

    public async Task<string> SaveBookImageAsync(
        IFormFile image,
        CancellationToken cancellationToken = default)
    {
        if (image.Length == 0)
        {
            throw new InvalidOperationException("Tệp ảnh không có nội dung.");
        }

        if (image.Length > MaxFileSize)
        {
            throw new InvalidOperationException("Ảnh bìa không được vượt quá 2 MB.");
        }

        var extension = Path.GetExtension(image.FileName);
        if (!AllowedExtensions.Contains(extension))
        {
            throw new InvalidOperationException("Ảnh bìa chỉ chấp nhận định dạng .jpg, .jpeg hoặc .png.");
        }

        var fileName = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
        var imageDirectory = Path.Combine(environment.WebRootPath, "images", "books");
        Directory.CreateDirectory(imageDirectory);

        var destinationPath = Path.Combine(imageDirectory, fileName);
        await using var stream = new FileStream(destinationPath, FileMode.CreateNew);
        await image.CopyToAsync(stream, cancellationToken);

        return $"/images/books/{fileName}";
    }

    public void DeleteBookImage(string? relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath) ||
            !relativePath.StartsWith("/images/books/", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var fileName = Path.GetFileName(relativePath);
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return;
        }

        var fullPath = Path.Combine(environment.WebRootPath, "images", "books", fileName);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }
}
