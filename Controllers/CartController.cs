using Microsoft.AspNetCore.Mvc;
using MVC_nhaSach.Services;
using MVC_nhaSach.ViewModels.Cart;

namespace MVC_nhaSach.Controllers;

public class CartController(ICartService cartService) : Controller
{
    [HttpGet]
    public IActionResult Index() => View(new CartViewModel { Items = cartService.GetItems() });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int bookId, int quantity = 1, string? returnUrl = null)
    {
        try
        {
            await cartService.AddAsync(bookId, quantity, HttpContext.RequestAborted);
            TempData["SuccessMessage"] = "Đã thêm sách vào giỏ hàng.";
        }
        catch (CartException exception)
        {
            TempData["ErrorMessage"] = exception.Message;
        }

        return !string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl)
            ? LocalRedirect(returnUrl)
            : RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int bookId, int quantity)
    {
        try
        {
            await cartService.UpdateQuantityAsync(bookId, quantity, HttpContext.RequestAborted);
            TempData["SuccessMessage"] = "Đã cập nhật giỏ hàng.";
        }
        catch (CartException exception)
        {
            TempData["ErrorMessage"] = exception.Message;
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Remove(int bookId)
    {
        cartService.Remove(bookId);
        TempData["SuccessMessage"] = "Đã xóa sản phẩm khỏi giỏ hàng.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Clear()
    {
        cartService.Clear();
        TempData["SuccessMessage"] = "Đã xóa toàn bộ giỏ hàng.";
        return RedirectToAction(nameof(Index));
    }
}
