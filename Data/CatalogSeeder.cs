using Microsoft.EntityFrameworkCore;
using MVC_nhaSach.Models;
using MVC_nhaSach.Models.Enums;

namespace MVC_nhaSach.Data;

public static class CatalogSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await using var transaction = await db.Database.BeginTransactionAsync();

        // Dọn đúng các bản ghi sinh ra bởi smoke test cũ; không đụng dữ liệu người dùng thật.
        var testOrders = await db.Orders
            .Where(order => order.CustomerName == "Khach Test" || order.CustomerName == "Cancel Test")
            .ToListAsync();
        db.Orders.RemoveRange(testOrders);
        await db.SaveChangesAsync();

        var testBooks = await db.Books
            .Where(book => book.Title.StartsWith("TransactionBook-"))
            .ToListAsync();
        db.Books.RemoveRange(testBooks);
        await db.SaveChangesAsync();

        var testCategories = await db.Categories
            .Where(category => category.Name.StartsWith("CheckoutTest-"))
            .ToListAsync();
        db.Categories.RemoveRange(testCategories);
        await db.SaveChangesAsync();

        var categories = new[]
        {
            new Category { Name = "Văn học", Description = "Tiểu thuyết, truyện ngắn và những tác phẩm sống cùng năm tháng." },
            new Category { Name = "Kinh tế", Description = "Kinh doanh, quản trị và tư duy xây dựng sản phẩm." },
            new Category { Name = "Kỹ năng sống", Description = "Những chỉ dẫn thực tế để làm việc và sống chủ động hơn." },
            new Category { Name = "Tâm lý", Description = "Hiểu cách con người suy nghĩ, cảm nhận và đưa ra quyết định." },
            new Category { Name = "Thiếu nhi", Description = "Những cuộc phiêu lưu nuôi dưỡng trí tưởng tượng của bạn đọc nhỏ." },
            new Category { Name = "Lịch sử", Description = "Nhìn lại quá khứ để hiểu rõ hơn thế giới hiện tại." },
            new Category { Name = "Khoa học", Description = "Các ý tưởng khoa học lớn được kể theo cách gần gũi." },
            new Category { Name = "Công nghệ", Description = "Lập trình, kiến trúc phần mềm và hành trang nghề nghiệp số." }
        };

        var existingCategoryNames = await db.Categories.Select(category => category.Name).ToHashSetAsync();
        db.Categories.AddRange(categories.Where(category => !existingCategoryNames.Contains(category.Name)));
        await db.SaveChangesAsync();

        var categoryIds = await db.Categories.ToDictionaryAsync(category => category.Name, category => category.Id);
        var now = DateTime.UtcNow;
        var books = new[]
        {
            NewBook("Nhà Giả Kim", "Paulo Coelho", 99000, 24, "Văn học", "/images/covers/nha-gia-kim.svg", true, "Một hành trình giản dị về ước mơ, lựa chọn và việc lắng nghe tiếng nói bên trong.", now.AddDays(-1)),
            NewBook("Truyện Kiều", "Nguyễn Du", 78000, 18, "Văn học", "/images/covers/truyen-kieu.svg", true, "Kiệt tác thơ Nôm của văn học Việt Nam với giá trị ngôn ngữ và nhân văn bền vững.", now.AddDays(-2)),
            NewBook("Cho Tôi Xin Một Vé Đi Tuổi Thơ", "Nguyễn Nhật Ánh", 90000, 31, "Văn học", "/images/covers/ve-di-tuoi-tho.svg", true, "Một chuyến tàu hóm hỉnh đưa người đọc trở lại thế giới trong veo của tuổi thơ.", now.AddDays(-3)),
            NewBook("Dế Mèn Phiêu Lưu Ký", "Tô Hoài", 85000, 27, "Thiếu nhi", "/images/covers/de-men.svg", true, "Cuộc trưởng thành qua những chuyến đi, tình bạn và bài học về trách nhiệm.", now.AddDays(-4)),
            NewBook("Đắc Nhân Tâm", "Dale Carnegie", 86000, 40, "Kỹ năng sống", "/images/covers/dac-nhan-tam.svg", false, "Các nguyên tắc giao tiếp kinh điển được trình bày bằng những câu chuyện dễ nhớ.", now.AddDays(-5)),
            NewBook("Tuổi Trẻ Đáng Giá Bao Nhiêu", "Rosie Nguyễn", 110000, 16, "Kỹ năng sống", "/images/covers/tuoi-tre.svg", true, "Gợi ý thực tế về học tập, trải nghiệm và xây dựng một tuổi trẻ có định hướng.", now.AddDays(-6)),
            NewBook("Tư Duy Nhanh Và Chậm", "Daniel Kahneman", 189000, 12, "Tâm lý", "/images/covers/tu-duy.svg", false, "Khám phá hai hệ thống tư duy chi phối phán đoán và quyết định của con người.", now.AddDays(-7)),
            NewBook("Lược Sử Loài Người", "Yuval Noah Harari", 209000, 14, "Lịch sử", "/images/covers/luoc-su.svg", true, "Một góc nhìn rộng về hành trình của Homo sapiens từ thời tiền sử đến hiện đại.", now.AddDays(-8)),
            NewBook("Vũ Trụ Trong Vỏ Hạt Dẻ", "Stephen Hawking", 175000, 9, "Khoa học", "/images/covers/vu-tru.svg", false, "Những khái niệm lớn về không gian, thời gian và vũ trụ qua lối giải thích trực quan.", now.AddDays(-9)),
            NewBook("Clean Code", "Robert C. Martin", 320000, 11, "Công nghệ", "/images/covers/clean-code.svg", true, "Nguyên tắc và thực hành giúp mã nguồn rõ ràng, dễ bảo trì và đáng tin cậy hơn.", now.AddDays(-10)),
            NewBook("Khởi Nghiệp Tinh Gọn", "Eric Ries", 145000, 20, "Kinh tế", "/images/covers/khoi-nghiep.svg", false, "Phương pháp thử nghiệm nhanh, học từ khách hàng và giảm lãng phí khi xây dựng sản phẩm.", now.AddDays(-11)),
            NewBook("Muôn Kiếp Nhân Sinh", "Nguyên Phong", 168000, 15, "Tâm lý", "/images/covers/muon-kiep.svg", false, "Những suy ngẫm về nhân quả, lựa chọn và trách nhiệm của con người với cuộc sống.", now.AddDays(-12)),
            NewBook("Số Đỏ", "Vũ Trọng Phụng", 92000, 22, "Văn học", "/images/covers/so-do.svg", true, "Một tác phẩm trào phúng sắc sảo về xã hội thành thị Việt Nam đầu thế kỷ XX.", now.AddDays(-13)),
            NewBook("Rừng Na Uy", "Haruki Murakami", 128000, 17, "Văn học", "/images/covers/rung-na-uy.svg", false, "Câu chuyện trưởng thành, tình yêu và mất mát được kể bằng giọng văn lặng lẽ.", now.AddDays(-14)),
            NewBook("Cây Cam Ngọt Của Tôi", "José Mauro de Vasconcelos", 108000, 26, "Văn học", "/images/covers/cay-cam-ngot.svg", true, "Một tuổi thơ vừa trong trẻo vừa nhiều tổn thương, được nâng đỡ bởi trí tưởng tượng.", now.AddDays(-15)),
            NewBook("Hoàng Tử Bé", "Antoine de Saint-Exupéry", 75000, 35, "Thiếu nhi", "/images/covers/hoang-tu-be.svg", true, "Câu chuyện nhỏ dành cho cả trẻ em và người lớn về tình bạn, trách nhiệm và yêu thương.", now.AddDays(-16)),
            NewBook("Tâm Lý Học Về Tiền", "Morgan Housel", 155000, 19, "Tâm lý", "/images/covers/tam-ly-tien.svg", false, "Những bài học về hành vi, cảm xúc và quyết định tài chính trong đời sống.", now.AddDays(-17)),
            NewBook("Một Đời Như Kẻ Tìm Đường", "Phan Văn Trường", 135000, 13, "Kỹ năng sống", "/images/covers/tim-duong.svg", false, "Các trải nghiệm nghề nghiệp và lựa chọn sống được kể bằng góc nhìn điềm tĩnh, thực tế.", now.AddDays(-18)),
            NewBook("Factfulness", "Hans Rosling", 179000, 10, "Khoa học", "/images/covers/factfulness.svg", true, "Dữ liệu và mười bản năng khiến chúng ta thường nhìn thế giới bi quan hơn thực tế.", now.AddDays(-19)),
            NewBook("Designing Data-Intensive Applications", "Martin Kleppmann", 420000, 8, "Công nghệ", "/images/covers/data-intensive.svg", false, "Nền tảng thiết kế hệ thống dữ liệu đáng tin cậy, có khả năng mở rộng và dễ bảo trì.", now.AddDays(-20)),
            NewBook("Chiến Lược Đại Dương Xanh", "W. Chan Kim & Renée Mauborgne", 188000, 14, "Kinh tế", "/images/covers/dai-duong-xanh.svg", true, "Phương pháp tạo không gian thị trường mới thay vì cạnh tranh trong những giới hạn cũ.", now.AddDays(-21)),
            NewBook("Việt Nam Sử Lược", "Trần Trọng Kim", 160000, 12, "Lịch sử", "/images/covers/viet-nam-su-luoc.svg", false, "Một công trình lịch sử Việt Nam có hệ thống, phù hợp để đọc và tra cứu tổng quan.", now.AddDays(-22))
        };

        var existingBookTitles = await db.Books.Select(book => book.Title).ToHashSetAsync();
        foreach (var book in books.Where(book => !existingBookTitles.Contains(book.Title)))
        {
            book.CategoryId = categoryIds[book.Category.Name];
            book.Category = null!;
            db.Books.Add(book);
        }

        await db.SaveChangesAsync();

        if (!await db.Orders.AnyAsync())
        {
            var customer = await db.Users.SingleOrDefaultAsync(user => user.Email == "customer@nhasach.local");
            if (customer is not null)
            {
                var bookLookup = await db.Books.ToDictionaryAsync(book => book.Title);
                var orders = new[]
                {
                    NewOrder(customer.Id, now.AddMonths(-5).AddDays(-4), OrderStatus.Completed, bookLookup,
                        ("Nhà Giả Kim", 1), ("Đắc Nhân Tâm", 1)),
                    NewOrder(customer.Id, now.AddMonths(-4).AddDays(-7), OrderStatus.Completed, bookLookup,
                        ("Clean Code", 1), ("Tư Duy Nhanh Và Chậm", 1)),
                    NewOrder(customer.Id, now.AddMonths(-3).AddDays(-2), OrderStatus.Completed, bookLookup,
                        ("Dế Mèn Phiêu Lưu Ký", 2), ("Hoàng Tử Bé", 1)),
                    NewOrder(customer.Id, now.AddMonths(-2).AddDays(-8), OrderStatus.Completed, bookLookup,
                        ("Lược Sử Loài Người", 1), ("Factfulness", 1)),
                    NewOrder(customer.Id, now.AddMonths(-1).AddDays(-3), OrderStatus.Completed, bookLookup,
                        ("Tuổi Trẻ Đáng Giá Bao Nhiêu", 2), ("Một Đời Như Kẻ Tìm Đường", 1)),
                    NewOrder(customer.Id, now.AddDays(-12), OrderStatus.Cancelled, bookLookup,
                        ("Rừng Na Uy", 1)),
                    NewOrder(customer.Id, now.AddDays(-6), OrderStatus.Shipping, bookLookup,
                        ("Cây Cam Ngọt Của Tôi", 1), ("Số Đỏ", 1)),
                    NewOrder(customer.Id, now.AddDays(-2), OrderStatus.Confirmed, bookLookup,
                        ("Chiến Lược Đại Dương Xanh", 1)),
                    NewOrder(customer.Id, now.AddHours(-8), OrderStatus.Pending, bookLookup,
                        ("Vũ Trụ Trong Vỏ Hạt Dẻ", 1), ("Việt Nam Sử Lược", 1))
                };
                db.Orders.AddRange(orders);
                await db.SaveChangesAsync();
            }
        }

        await transaction.CommitAsync();
    }

    private static Book NewBook(string title, string author, decimal price, int stock, string category,
        string imagePath, bool featured, string description, DateTime createdDate) => new()
        {
            Title = title,
            Author = author,
            Price = price,
            StockQuantity = stock,
            Category = new Category { Name = category },
            ImagePath = imagePath,
            IsFeatured = featured,
            Description = description,
            CreatedDate = createdDate
        };

    private static Order NewOrder(string userId, DateTime orderDate, OrderStatus status,
        IReadOnlyDictionary<string, Book> books, params (string Title, int Quantity)[] items)
    {
        var details = items.Select(item => new OrderDetail
        {
            BookId = books[item.Title].Id,
            Quantity = item.Quantity,
            UnitPrice = books[item.Title].Price
        }).ToList();

        return new Order
        {
            UserId = userId,
            CustomerName = "Nguyễn Minh Anh",
            Phone = "0908 123 456",
            Address = "72 Nguyễn Thị Minh Khai, Quận 3, TP. Hồ Chí Minh",
            OrderDate = orderDate,
            Status = status,
            IsStockRestored = status == OrderStatus.Cancelled,
            TotalAmount = details.Sum(detail => detail.Quantity * detail.UnitPrice),
            OrderDetails = details
        };
    }
}
