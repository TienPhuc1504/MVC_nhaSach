using MVC_nhaSach.Models;

namespace MVC_nhaSach.ViewModels.Cart;

public class CartViewModel
{
    public IReadOnlyList<CartItem> Items { get; set; } = [];
    public int TotalQuantity => Items.Sum(item => item.Quantity);
    public decimal TotalAmount => Items.Sum(item => item.LineTotal);
}
