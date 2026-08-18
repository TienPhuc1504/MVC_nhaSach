using MVC_nhaSach.Models;

namespace MVC_nhaSach.Services;

public interface ICartService
{
    IReadOnlyList<CartItem> GetItems();
    int GetTotalQuantity();
    Task AddAsync(int bookId, int quantity, CancellationToken cancellationToken = default);
    Task UpdateQuantityAsync(int bookId, int quantity, CancellationToken cancellationToken = default);
    void Remove(int bookId);
    void Clear();
}
