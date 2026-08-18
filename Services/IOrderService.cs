using MVC_nhaSach.Models;
using MVC_nhaSach.ViewModels.Orders;
using MVC_nhaSach.Models.Enums;

namespace MVC_nhaSach.Services;

public interface IOrderService
{
    Task<int> PlaceOrderAsync(
        string userId,
        CheckoutViewModel checkout,
        IReadOnlyList<CartItem> cartItems,
        CancellationToken cancellationToken = default);

    Task UpdateStatusAsync(
        int orderId,
        OrderStatus newStatus,
        CancellationToken cancellationToken = default);
}
