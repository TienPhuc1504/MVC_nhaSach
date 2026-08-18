using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_nhaSach.Areas.Admin.ViewModels;
using MVC_nhaSach.Data;
using MVC_nhaSach.Models;

namespace MVC_nhaSach.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = IdentitySeeder.AdminRole)]
public class UsersController(UserManager<ApplicationUser> userManager) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var users = await userManager.Users.AsNoTracking().OrderBy(user => user.Email).ToListAsync();
        var models = new List<UserRoleViewModel>();
        foreach (var user in users)
        {
            models.Add(new UserRoleViewModel
            {
                Id = user.Id,
                Email = user.Email ?? user.UserName ?? "(không có email)",
                Roles = (await userManager.GetRolesAsync(user)).ToList()
            });
        }
        return View(models);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetAdmin(string id, bool makeAdmin)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user is null)
        {
            return NotFound();
        }

        var currentUserId = userManager.GetUserId(User);
        if (!makeAdmin && user.Id == currentUserId)
        {
            TempData["ErrorMessage"] = "Bạn không thể tự gỡ quyền Admin của chính mình.";
            return RedirectToAction(nameof(Index));
        }

        var isAdmin = await userManager.IsInRoleAsync(user, IdentitySeeder.AdminRole);
        IdentityResult result;
        if (makeAdmin && !isAdmin)
        {
            result = await userManager.AddToRoleAsync(user, IdentitySeeder.AdminRole);
        }
        else if (!makeAdmin && isAdmin)
        {
            var adminCount = (await userManager.GetUsersInRoleAsync(IdentitySeeder.AdminRole)).Count;
            if (adminCount <= 1)
            {
                TempData["ErrorMessage"] = "Hệ thống phải còn ít nhất một tài khoản Admin.";
                return RedirectToAction(nameof(Index));
            }
            result = await userManager.RemoveFromRoleAsync(user, IdentitySeeder.AdminRole);
        }
        else
        {
            return RedirectToAction(nameof(Index));
        }

        TempData[result.Succeeded ? "SuccessMessage" : "ErrorMessage"] = result.Succeeded
            ? "Cập nhật quyền người dùng thành công."
            : string.Join("; ", result.Errors.Select(error => error.Description));
        return RedirectToAction(nameof(Index));
    }
}
