namespace MVC_nhaSach.Services;

public class ImageService(IWebHostEnvironment environment) : IImageService
{
    public const long MaxFileSize = 2 * 1024 * 1024;
    public const long MaxTeamBackgroundFileSize = 8 * 1024 * 1024;
    private static readonly HashSet<string> AllowedExtensions =
        new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".webp" };

    public async Task<string> SaveBookImageAsync(
        IFormFile image,
        CancellationToken cancellationToken = default)
        => await SaveImageAsync(image, "books", "Ảnh bìa", MaxFileSize, cancellationToken);

    public void DeleteBookImage(string? relativePath)
        => DeleteImage(relativePath, "books");

    public async Task<string> SaveTeamMemberBackgroundAsync(
        IFormFile image,
        CancellationToken cancellationToken = default)
        => await SaveImageAsync(
            image,
            "team",
            "Ảnh nền thành viên",
            MaxTeamBackgroundFileSize,
            cancellationToken);

    public void DeleteTeamMemberBackground(string? relativePath)
        => DeleteImage(relativePath, "team");

    private async Task<string> SaveImageAsync(
        IFormFile image,
        string directoryName,
        string imageLabel,
        long maxFileSize,
        CancellationToken cancellationToken)
    {
        if (image.Length == 0)
        {
            throw new InvalidOperationException("Tệp ảnh không có nội dung.");
        }

        if (image.Length > maxFileSize)
        {
            var maxFileSizeInMb = maxFileSize / (1024 * 1024);
            throw new InvalidOperationException($"{imageLabel} không được vượt quá {maxFileSizeInMb} MB.");
        }

        var extension = Path.GetExtension(image.FileName);
        if (!AllowedExtensions.Contains(extension))
        {
            throw new InvalidOperationException($"{imageLabel} chỉ chấp nhận định dạng JPG, JPEG, PNG hoặc WebP.");
        }

        var fileName = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
        var imageDirectory = Path.Combine(UploadStorage.RootPath, directoryName);
        Directory.CreateDirectory(imageDirectory);

        var destinationPath = Path.Combine(imageDirectory, fileName);
        await using var stream = new FileStream(destinationPath, FileMode.CreateNew);
        await image.CopyToAsync(stream, cancellationToken);

        return $"{UploadStorage.RequestPath}/{directoryName}/{fileName}";
    }

    private void DeleteImage(string? relativePath, string directoryName)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            return;
        }

        var fileName = Path.GetFileName(relativePath);
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return;
        }

        string? fullPath = null;
        var uploadPrefix = $"{UploadStorage.RequestPath}/{directoryName}/";
        var legacyPrefix = $"/images/{directoryName}/";
        if (relativePath.StartsWith(uploadPrefix, StringComparison.OrdinalIgnoreCase))
        {
            fullPath = Path.Combine(UploadStorage.RootPath, directoryName, fileName);
        }
        else if (relativePath.StartsWith(legacyPrefix, StringComparison.OrdinalIgnoreCase))
        {
            fullPath = Path.Combine(environment.WebRootPath, "images", directoryName, fileName);
        }

        if (fullPath is null)
        {
            return;
        }

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }
}
