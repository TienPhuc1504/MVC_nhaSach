using System.Data;
using Microsoft.EntityFrameworkCore;
using MVC_nhaSach.Data;
using MVC_nhaSach.Models;
using MVC_nhaSach.Models.Enums;
using MVC_nhaSach.ViewModels.Orders;

namespace MVC_nhaSach.Services;

public class OrderService(ApplicationDbContext context) : IOrderService
{
    public async Task<int> PlaceOrderAsync(
        string userId,
        CheckoutViewModel checkout,
        IReadOnlyList<CartItem> cartItems,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new OrderException("Không xác định được người đặt hàng.");
        }
        if (cartItems.Count == 0)
        {
            throw new OrderException("Giỏ hàng đang trống.");
        }

        var requestedItems = cartItems
            .GroupBy(item => item.BookId)
            .ToDictionary(group => group.Key, group => group.Sum(item => item.Quantity));
        if (requestedItems.Values.Any(quantity => quantity < 1))
        {
            throw new OrderException("Số lượng sản phẩm không hợp lệ.");
        }

        await using var transaction = await context.Database.BeginTransactionAsync(
            IsolationLevel.Serializable, cancellationToken);
        try
        {
            var bookIds = requestedItems.Keys.ToList();
            var books = await context.Books.Where(book => bookIds.Contains(book.Id))
                .ToDictionaryAsync(book => book.Id, cancellationToken);
            if (books.Count != requestedItems.Count)
            {
                throw new OrderException("Một hoặc nhiều sách trong giỏ không còn tồn tại.");
            }

            foreach (var requested in requestedItems)
            {
                var book = books[requested.Key];
                if (requested.Value > book.StockQuantity)
                {
                    throw new OrderException($"Sách “{book.Title}” chỉ còn {book.StockQuantity} sản phẩm.");
                }
            }

            var order = new Order
            {
                CustomerName = checkout.CustomerName.Trim(),
                Phone = checkout.Phone.Trim(),
                Address = checkout.Address.Trim(),
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                UserId = userId
            };

            foreach (var requested in requestedItems)
            {
                var book = books[requested.Key];
                book.StockQuantity -= requested.Value;
                order.OrderDetails.Add(new OrderDetail
                {
                    BookId = book.Id,
                    Quantity = requested.Value,
                    UnitPrice = book.Price
                });
            }
            order.TotalAmount = order.OrderDetails.Sum(detail => detail.Quantity * detail.UnitPrice);

            context.Orders.Add(order);
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return order.Id;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task UpdateStatusAsync(
        int orderId,
        OrderStatus newStatus,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(
            IsolationLevel.Serializable, cancellationToken);
        try
        {
            var order = await context.Orders
                .Include(item => item.OrderDetails)
                .ThenInclude(detail => detail.Book)
                .FirstOrDefaultAsync(item => item.Id == orderId, cancellationToken)
                ?? throw new OrderException("Đơn hàng không tồn tại.");

            if (order.Status == newStatus)
            {
                await transaction.CommitAsync(cancellationToken);
                return;
            }

            if (!IsValidTransition(order.Status, newStatus))
            {
                throw new OrderException($"Không thể chuyển trạng thái từ {order.Status} sang {newStatus}.");
            }

            if (newStatus == OrderStatus.Cancelled && !order.IsStockRestored)
            {
                foreach (var detail in order.OrderDetails)
                {
                    detail.Book.StockQuantity += detail.Quantity;
                }
                order.IsStockRestored = true;
            }

            order.Status = newStatus;
            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private static bool IsValidTransition(OrderStatus current, OrderStatus next) => current switch
    {
        OrderStatus.Pending => next is OrderStatus.Confirmed or OrderStatus.Cancelled,
        OrderStatus.Confirmed => next is OrderStatus.Shipping or OrderStatus.Cancelled,
        OrderStatus.Shipping => next is OrderStatus.Completed or OrderStatus.Cancelled,
        _ => false
    };
}
