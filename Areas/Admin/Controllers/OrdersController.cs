using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_nhaSach.Areas.Admin.ViewModels;
using MVC_nhaSach.Data;
using MVC_nhaSach.Models.Enums;
using MVC_nhaSach.Services;

namespace MVC_nhaSach.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = IdentitySeeder.AdminRole)]
public class OrdersController(ApplicationDbContext context, IOrderService orderService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(OrderListViewModel model)
    {
        var query = context.Orders.AsNoTracking().Include(order => order.OrderDetails).AsQueryable();
        if (!string.IsNullOrWhiteSpace(model.Customer))
        {
            var customer = model.Customer.Trim();
            query = query.Where(order => order.CustomerName.Contains(customer) || order.Phone.Contains(customer));
        }
        if (model.Status.HasValue)
        {
            query = query.Where(order => order.Status == model.Status.Value);
        }
        model.Orders = await query.OrderByDescending(order => order.OrderDate).ToListAsync();
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var order = await context.Orders.AsNoTracking()
            .Include(item => item.User)
            .Include(item => item.OrderDetails).ThenInclude(detail => detail.Book)
            .FirstOrDefaultAsync(item => item.Id == id);
        return order is null ? NotFound() : View(order);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, OrderStatus status)
    {
        if (!Enum.IsDefined(status))
        {
            TempData["ErrorMessage"] = "Trạng thái không hợp lệ.";
            return RedirectToAction(nameof(Details), new { id });
        }

        try
        {
            await orderService.UpdateStatusAsync(id, status, HttpContext.RequestAborted);
            TempData["SuccessMessage"] = "Cập nhật trạng thái đơn hàng thành công.";
        }
        catch (OrderException exception)
        {
            TempData["ErrorMessage"] = exception.Message;
        }
        return RedirectToAction(nameof(Details), new { id });
    }
}
