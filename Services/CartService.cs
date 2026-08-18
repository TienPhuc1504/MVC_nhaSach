using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MVC_nhaSach.Data;
using MVC_nhaSach.Models;

namespace MVC_nhaSach.Services;

public class CartService(IHttpContextAccessor httpContextAccessor, ApplicationDbContext context) : ICartService
{
    private const string CartKey = "ShoppingCart";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private ISession Session => httpContextAccessor.HttpContext?.Session
        ?? throw new InvalidOperationException("Session không khả dụng.");

    public IReadOnlyList<CartItem> GetItems() => ReadItems();
    public int GetTotalQuantity() => ReadItems().Sum(item => item.Quantity);

    public async Task AddAsync(int bookId, int quantity, CancellationToken cancellationToken = default)
    {
        if (quantity < 1)
        {
            throw new CartException("Số lượng phải ít nhất là 1.");
        }

        var book = await context.Books.AsNoTracking().FirstOrDefaultAsync(item => item.Id == bookId, cancellationToken)
            ?? throw new CartException("Sách không tồn tại.");
        if (book.StockQuantity < 1)
        {
            throw new CartException("Sách hiện đã hết hàng.");
        }

        var items = ReadItems();
        var existing = items.FirstOrDefault(item => item.BookId == bookId);
        var newQuantity = quantity + (existing?.Quantity ?? 0);
        if (newQuantity > book.StockQuantity)
        {
            throw new CartException($"Chỉ còn {book.StockQuantity} sản phẩm trong kho.");
        }

        if (existing is null)
        {
            items.Add(new CartItem
            {
                BookId = book.Id,
                Title = book.Title,
                Author = book.Author,
                ImagePath = book.ImagePath,
                UnitPrice = book.Price,
                Quantity = quantity,
                StockQuantity = book.StockQuantity
            });
        }
        else
        {
            existing.Quantity = newQuantity;
            existing.StockQuantity = book.StockQuantity;
            existing.UnitPrice = book.Price;
        }

        WriteItems(items);
    }

    public async Task UpdateQuantityAsync(int bookId, int quantity, CancellationToken cancellationToken = default)
    {
        if (quantity < 1)
        {
            throw new CartException("Số lượng phải ít nhất là 1. Hãy dùng nút xóa nếu không muốn mua sách này.");
        }

        var items = ReadItems();
        var item = items.FirstOrDefault(cartItem => cartItem.BookId == bookId)
            ?? throw new CartException("Sản phẩm không có trong giỏ hàng.");
        var book = await context.Books.AsNoTracking().FirstOrDefaultAsync(book => book.Id == bookId, cancellationToken)
            ?? throw new CartException("Sách không còn tồn tại.");
        if (quantity > book.StockQuantity)
        {
            throw new CartException($"Chỉ còn {book.StockQuantity} sản phẩm trong kho.");
        }

        item.Quantity = quantity;
        item.StockQuantity = book.StockQuantity;
        item.UnitPrice = book.Price;
        WriteItems(items);
    }

    public void Remove(int bookId)
    {
        var items = ReadItems();
        items.RemoveAll(item => item.BookId == bookId);
        WriteItems(items);
    }

    public void Clear() => Session.Remove(CartKey);

    private List<CartItem> ReadItems()
    {
        var json = Session.GetString(CartKey);
        return string.IsNullOrWhiteSpace(json)
            ? []
            : JsonSerializer.Deserialize<List<CartItem>>(json, JsonOptions) ?? [];
    }

    private void WriteItems(List<CartItem> items)
    {
        if (items.Count == 0)
        {
            Clear();
            return;
        }
        Session.SetString(CartKey, JsonSerializer.Serialize(items, JsonOptions));
    }
}
