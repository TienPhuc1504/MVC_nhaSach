using MVC_nhaSach.Models;

namespace MVC_nhaSach.Areas.Admin.ViewModels;

public class DashboardViewModel
{
    public decimal TotalRevenue { get; set; }
    public int TotalOrders { get; set; }
    public int TotalBooks { get; set; }
    public int LowStockBooksCount { get; set; }
    public int TotalUsers { get; set; }
    public IReadOnlyList<TopSellingBookViewModel> TopSellingBooks { get; set; } = [];
    public IReadOnlyList<Order> LatestOrders { get; set; } = [];
    public IReadOnlyList<Book> LowStockBooks { get; set; } = [];
    public IReadOnlyList<MonthlyRevenueViewModel> RevenueByMonth { get; set; } = [];
}

public class TopSellingBookViewModel
{
    public int BookId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int QuantitySold { get; set; }
    public decimal Revenue { get; set; }
}

public class MonthlyRevenueViewModel
{
    public string Label { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
}
