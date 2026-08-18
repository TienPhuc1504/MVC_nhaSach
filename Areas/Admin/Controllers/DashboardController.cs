using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_nhaSach.Areas.Admin.ViewModels;
using MVC_nhaSach.Data;
using MVC_nhaSach.Models;
using MVC_nhaSach.Models.Enums;

namespace MVC_nhaSach.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = IdentitySeeder.AdminRole)]
public class DashboardController(
    ApplicationDbContext context,
    UserManager<ApplicationUser> userManager) : Controller
{
    private const int LowStockThreshold = 5;

    public async Task<IActionResult> Index()
    {
        var now = DateTime.UtcNow;
        var firstMonth = new DateTime(now.Year, now.Month, 1).AddMonths(-5);

        var monthlyData = await context.Orders.AsNoTracking()
            .Where(order => order.Status == OrderStatus.Completed && order.OrderDate >= firstMonth)
            .GroupBy(order => new { order.OrderDate.Year, order.OrderDate.Month })
            .Select(group => new
            {
                group.Key.Year,
                group.Key.Month,
                Revenue = group.Sum(order => order.TotalAmount)
            })
            .ToListAsync();

        var revenueByMonth = Enumerable.Range(0, 6)
            .Select(offset => firstMonth.AddMonths(offset))
            .Select(month => new MonthlyRevenueViewModel
            {
                Label = $"{month.Month:00}/{month.Year}",
                Revenue = monthlyData.FirstOrDefault(item => item.Year == month.Year && item.Month == month.Month)?.Revenue ?? 0
            })
            .ToList();

        var model = new DashboardViewModel
        {
            TotalRevenue = await context.Orders.AsNoTracking()
                .Where(order => order.Status == OrderStatus.Completed)
                .SumAsync(order => (decimal?)order.TotalAmount) ?? 0,
            TotalOrders = await context.Orders.CountAsync(),
            TotalBooks = await context.Books.CountAsync(),
            LowStockBooksCount = await context.Books.CountAsync(book => book.StockQuantity <= LowStockThreshold),
            TotalUsers = await userManager.Users.CountAsync(),
            TopSellingBooks = await context.OrderDetails.AsNoTracking()
                .Where(detail => detail.Order.Status == OrderStatus.Completed)
                .GroupBy(detail => new { detail.BookId, detail.Book.Title })
                .Select(group => new TopSellingBookViewModel
                {
                    BookId = group.Key.BookId,
                    Title = group.Key.Title,
                    QuantitySold = group.Sum(detail => detail.Quantity),
                    Revenue = group.Sum(detail => detail.Quantity * detail.UnitPrice)
                })
                .OrderByDescending(item => item.QuantitySold)
                .Take(5)
                .ToListAsync(),
            LatestOrders = await context.Orders.AsNoTracking()
                .Include(order => order.OrderDetails)
                .OrderByDescending(order => order.OrderDate)
                .Take(5)
                .ToListAsync(),
            LowStockBooks = await context.Books.AsNoTracking()
                .Include(book => book.Category)
                .Where(book => book.StockQuantity <= LowStockThreshold)
                .OrderBy(book => book.StockQuantity)
                .ThenBy(book => book.Title)
                .Take(5)
                .ToListAsync(),
            RevenueByMonth = revenueByMonth
        };

        return View(model);
    }
}
