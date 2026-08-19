using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_nhaSach.Data;
using MVC_nhaSach.Models;
using MVC_nhaSach.Services;
using MVC_nhaSach.ViewModels.Orders;

namespace MVC_nhaSach.Controllers;

[Authorize]
public class OrdersController(
    ApplicationDbContext context,
    UserManager<ApplicationUser> userManager,
    ICartService cartService,
    IOrderService orderService,
    ILogger<OrdersController> logger) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = userManager.GetUserId(User)!;
        var orders = await context.Orders.AsNoTracking()
            .Where(order => order.UserId == userId)
            .Include(order => order.OrderDetails)
            .OrderByDescending(order => order.OrderDate)
            .ToListAsync();
        return View(orders);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var userId = userManager.GetUserId(User)!;
        var order = await context.Orders.AsNoTracking()
            .Where(item => item.Id == id && item.UserId == userId)
            .Include(item => item.OrderDetails)
            .ThenInclude(detail => detail.Book)
            .FirstOrDefaultAsync();
        return order is null ? NotFound() : View(order);
    }

    [HttpGet]
    public async Task<IActionResult> Checkout()
    {
        if (cartService.GetItems().Count == 0)
        {
            TempData["ErrorMessage"] = "Giỏ hàng đang trống.";
            return RedirectToAction("Index", "Cart");
        }

        await PopulateCheckoutViewDataAsync();
        return View(new CheckoutViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(CheckoutViewModel model)
    {
        if (model.DeliverToMyself)
        {
            var user = await userManager.GetUserAsync(User);
            model.CustomerName = user?.FullName ?? string.Empty;
            model.Phone = user?.PhoneNumber ?? string.Empty;
            model.Address = user?.ShippingAddress ?? string.Empty;
            ModelState.Clear();
            TryValidateModel(model);
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Hãy cập nhật đủ thông tin trong hồ sơ trước khi đặt cho bản thân.");
            }
        }

        var cartItems = cartService.GetItems();
        if (cartItems.Count == 0)
        {
            ModelState.AddModelError(string.Empty, "Giỏ hàng đang trống.");
        }
        if (!ModelState.IsValid)
        {
            await PopulateCheckoutViewDataAsync(cartItems);
            return View(model);
        }

        try
        {
            var userId = userManager.GetUserId(User)!;
            var orderId = await orderService.PlaceOrderAsync(
                userId, model, cartItems, HttpContext.RequestAborted);
            cartService.Clear();
            TempData["SuccessMessage"] = $"Đặt hàng thành công. Mã đơn hàng: #{orderId}.";
            return RedirectToAction(nameof(Details), new { id = orderId });
        }
        catch (OrderException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            await PopulateCheckoutViewDataAsync(cartItems);
            return View(model);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Không thể tạo đơn hàng cho người dùng {UserId}.", userManager.GetUserId(User));
            ModelState.AddModelError(string.Empty, "Không thể hoàn tất đơn hàng lúc này. Vui lòng thử lại.");
            await PopulateCheckoutViewDataAsync(cartItems);
            return View(model);
        }
    }

    private async Task PopulateCheckoutViewDataAsync(IReadOnlyList<CartItem>? cartItems = null)
    {
        var user = await userManager.GetUserAsync(User);
        ViewBag.CartItems = cartItems ?? cartService.GetItems();
        ViewBag.ProfileName = user?.FullName ?? string.Empty;
        ViewBag.ProfilePhone = user?.PhoneNumber ?? string.Empty;
        ViewBag.ProfileAddress = user?.ShippingAddress ?? string.Empty;
    }
}
