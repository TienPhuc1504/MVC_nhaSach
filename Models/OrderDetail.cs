using System.ComponentModel.DataAnnotations;

namespace MVC_nhaSach.Models;

public class OrderDetail
{
    public int OrderId { get; set; }
    public int BookId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải ít nhất là 1.")]
    [Display(Name = "Số lượng")]
    public int Quantity { get; set; }

    [Range(typeof(decimal), "0.01", "9999999999999999",
        ParseLimitsInInvariantCulture = true,
        ConvertValueInInvariantCulture = true,
        ErrorMessage = "Đơn giá phải lớn hơn 0.")]
    [Display(Name = "Đơn giá")]
    public decimal UnitPrice { get; set; }

    public Order Order { get; set; } = null!;
    public Book Book { get; set; } = null!;
}
