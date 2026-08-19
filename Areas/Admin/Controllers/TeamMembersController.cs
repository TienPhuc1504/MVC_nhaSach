using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_nhaSach.Data;
using MVC_nhaSach.Services;

namespace MVC_nhaSach.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = IdentitySeeder.AdminRole)]
public class TeamMembersController(
    ApplicationDbContext context,
    IImageService imageService,
    ILogger<TeamMembersController> logger) : Controller
{
    public async Task<IActionResult> Index()
    {
        var members = await context.TeamMembers.AsNoTracking()
            .OrderBy(member => member.SortOrder)
            .ToListAsync();
        return View(members);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(ImageService.MaxTeamBackgroundFileSize + 1024 * 1024)]
    public async Task<IActionResult> UpdateBackground(
        int id,
        IFormFile? backgroundImage,
        CancellationToken cancellationToken)
    {
        var member = await context.TeamMembers.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (member is null)
        {
            return NotFound();
        }

        if (backgroundImage is null)
        {
            TempData["ErrorMessage"] = "Vui lòng chọn một ảnh nền.";
            return RedirectToAction(nameof(Index));
        }

        string? newImagePath = null;
        string? oldImagePath = null;
        try
        {
            newImagePath = await imageService.SaveTeamMemberBackgroundAsync(backgroundImage, cancellationToken);
            oldImagePath = member.BackgroundImagePath;
            member.BackgroundImagePath = newImagePath;
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (InvalidOperationException exception)
        {
            TryDeleteBackground(newImagePath);
            TempData["ErrorMessage"] = exception.Message;
            return RedirectToAction(nameof(Index));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            TryDeleteBackground(newImagePath);
            logger.LogError(exception, "Không thể cập nhật ảnh nền cho thành viên {TeamMemberId}.", id);
            TempData["ErrorMessage"] = "Không thể lưu ảnh nền. Vui lòng thử lại hoặc chọn một ảnh khác.";
            return RedirectToAction(nameof(Index));
        }

        TryDeleteBackground(oldImagePath);

        TempData["SuccessMessage"] = $"Đã cập nhật ảnh nền của {member.FullName}.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveBackground(int id, CancellationToken cancellationToken)
    {
        var member = await context.TeamMembers.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (member is null)
        {
            return NotFound();
        }

        var imagePath = member.BackgroundImagePath;
        member.BackgroundImagePath = null;
        await context.SaveChangesAsync(cancellationToken);
        TryDeleteBackground(imagePath);

        TempData["SuccessMessage"] = $"Đã gỡ ảnh nền của {member.FullName}.";
        return RedirectToAction(nameof(Index));
    }

    private void TryDeleteBackground(string? imagePath)
    {
        try
        {
            imageService.DeleteTeamMemberBackground(imagePath);
        }
        catch (Exception exception)
        {
            logger.LogWarning(exception, "Không thể dọn tệp ảnh nền {ImagePath}.", imagePath);
        }
    }
}
