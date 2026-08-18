using System.ComponentModel.DataAnnotations;

namespace MVC_nhaSach.Models;

public class Book
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên sách.")]
    [StringLength(200, ErrorMessage = "Tên sách không được vượt quá 200 ký tự.")]
    [Display(Name = "Tên sách")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập tên tác giả.")]
    [StringLength(150, ErrorMessage = "Tên tác giả không được vượt quá 150 ký tự.")]
    [Display(Name = "Tác giả")]
    public string Author { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.01", "9999999999999999",
        ParseLimitsInInvariantCulture = true,
        ConvertValueInInvariantCulture = true,
        ErrorMessage = "Giá sách phải lớn hơn 0.")]
    [Display(Name = "Giá bán")]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn không được âm.")]
    [Display(Name = "Số lượng tồn")]
    public int StockQuantity { get; set; }

    [StringLength(2000, ErrorMessage = "Mô tả không được vượt quá 2000 ký tự.")]
    [Display(Name = "Mô tả")]
    public string? Description { get; set; }

    [StringLength(300)]
    [Display(Name = "Ảnh bìa")]
    public string? ImagePath { get; set; }

    [Display(Name = "Sách nổi bật")]
    public bool IsFeatured { get; set; }

    [Display(Name = "Ngày tạo")]
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn danh mục.")]
    [Display(Name = "Danh mục")]
    public int CategoryId { get; set; }

    public Category Category { get; set; } = null!;
    public ICollection<OrderDetail> OrderDetails { get; set; } = [];
}
