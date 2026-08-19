using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MVC_nhaSach.Data;
using MVC_nhaSach.Models;
using MVC_nhaSach.ViewModels.Account;

namespace MVC_nhaSach.Controllers;

public class AccountController(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager) : Controller
{
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = new ApplicationUser
        {
            UserName = model.Email.Trim(),
            Email = model.Email.Trim()
        };

        var createResult = await userManager.CreateAsync(user, model.Password);
        if (!createResult.Succeeded)
        {
            AddIdentityErrors(createResult);
            return View(model);
        }

        var roleResult = await userManager.AddToRoleAsync(user, IdentitySeeder.CustomerRole);
        if (!roleResult.Succeeded)
        {
            await userManager.DeleteAsync(user);
            AddIdentityErrors(roleResult);
            return View(model);
        }

        await signInManager.SignInAsync(user, isPersistent: false);
        TempData["SuccessMessage"] = "Đăng ký tài khoản thành công.";
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await signInManager.PasswordSignInAsync(
            model.Email.Trim(), model.Password, model.RememberMe, lockoutOnFailure: true);

        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.IsLockedOut
                ? "Tài khoản tạm thời bị khóa do đăng nhập sai nhiều lần."
                : "Email hoặc mật khẩu không đúng.");
            return View(model);
        }

        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return LocalRedirect(model.ReturnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Profile(bool edit = false)
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null) return Challenge();
        ViewBag.Email = user.Email;
        ViewBag.IsEditingProfile = edit;
        return View(new ProfileViewModel
        {
            FullName = user.FullName,
            Phone = user.PhoneNumber ?? string.Empty,
            Address = user.ShippingAddress
        });
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(ProfileViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var invalidUser = await userManager.GetUserAsync(User);
            ViewBag.Email = invalidUser?.Email;
            ViewBag.IsEditingProfile = true;
            return View(model);
        }

        var user = await userManager.GetUserAsync(User);
        if (user is null) return Challenge();
        user.FullName = model.FullName.Trim();
        user.PhoneNumber = model.Phone.Trim();
        user.ShippingAddress = model.Address.Trim();
        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            AddIdentityErrors(result);
            ViewBag.Email = user.Email;
            ViewBag.IsEditingProfile = true;
            return View(model);
        }

        TempData["SuccessMessage"] = "Đã cập nhật thông tin hồ sơ.";
        return RedirectToAction(nameof(Profile));
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult AccessDenied() => View();

    private void AddIdentityErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }
}
