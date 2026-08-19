namespace MVC_nhaSach.Services;

public interface IImageService
{
    Task<string> SaveBookImageAsync(IFormFile image, CancellationToken cancellationToken = default);
    void DeleteBookImage(string? relativePath);
    Task<string> SaveTeamMemberBackgroundAsync(IFormFile image, CancellationToken cancellationToken = default);
    void DeleteTeamMemberBackground(string? relativePath);
}
