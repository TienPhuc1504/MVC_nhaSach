using MVC_nhaSach.Models;
using MVC_nhaSach.Models.Enums;

namespace MVC_nhaSach.Areas.Admin.ViewModels;

public class OrderListViewModel
{
    public string? Customer { get; set; }
    public OrderStatus? Status { get; set; }
    public IReadOnlyList<Order> Orders { get; set; } = [];
}
