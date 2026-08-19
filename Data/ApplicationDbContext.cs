using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MVC_nhaSach.Models;

namespace MVC_nhaSach.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderDetail> OrderDetails => Set<OrderDetail>();
    public DbSet<TeamMember> TeamMembers => Set<TeamMember>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Category>()
            .HasIndex(category => category.Name)
            .IsUnique();

        builder.Entity<Category>()
            .HasMany(category => category.Books)
            .WithOne(book => book.Category)
            .HasForeignKey(book => book.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Book>()
            .Property(book => book.Price)
            .HasPrecision(18, 2);

        builder.Entity<Book>()
            .Property(book => book.CreatedDate)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Entity<TeamMember>()
            .HasIndex(member => member.StudentId)
            .IsUnique();

        builder.Entity<ApplicationUser>()
            .Property(user => user.FullName)
            .HasMaxLength(100);

        builder.Entity<ApplicationUser>()
            .Property(user => user.ShippingAddress)
            .HasMaxLength(300);

        builder.Entity<Order>()
            .Property(order => order.TotalAmount)
            .HasPrecision(18, 2);

        builder.Entity<Order>()
            .Property(order => order.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Entity<Order>()
            .Property(order => order.OrderDate)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Entity<Order>()
            .Property(order => order.Note)
            .HasMaxLength(500);

        builder.Entity<Order>()
            .HasOne(order => order.User)
            .WithMany(user => user.Orders)
            .HasForeignKey(order => order.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<OrderDetail>()
            .HasKey(detail => new { detail.OrderId, detail.BookId });

        builder.Entity<OrderDetail>()
            .Property(detail => detail.UnitPrice)
            .HasPrecision(18, 2);

        builder.Entity<OrderDetail>()
            .HasOne(detail => detail.Order)
            .WithMany(order => order.OrderDetails)
            .HasForeignKey(detail => detail.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<OrderDetail>()
            .HasOne(detail => detail.Book)
            .WithMany(book => book.OrderDetails)
            .HasForeignKey(detail => detail.BookId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
