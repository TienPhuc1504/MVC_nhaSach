using System.ComponentModel.DataAnnotations;
using MVC_nhaSach.Models.Enums;

namespace MVC_nhaSach.Models;

public class Order
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập họ tên người nhận.")]
    [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự.")]
    [Display(Name = "Người nhận")]
    public string CustomerName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
    [StringLength(20, MinimumLength = 8, ErrorMessage = "Số điện thoại phải có từ 8 đến 20 ký tự.")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
    [Display(Name = "Số điện thoại")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng.")]
    [StringLength(300, ErrorMessage = "Địa chỉ không được vượt quá 300 ký tự.")]
    [Display(Name = "Địa chỉ")]
    public string Address { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự.")]
    [Display(Name = "Ghi chú đơn hàng")]
    public string? Note { get; set; }

    [Display(Name = "Ngày đặt")]
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    [Range(typeof(decimal), "0", "9999999999999999",
        ParseLimitsInInvariantCulture = true,
        ConvertValueInInvariantCulture = true,
        ErrorMessage = "Tổng tiền không hợp lệ.")]
    [Display(Name = "Tổng tiền")]
    public decimal TotalAmount { get; set; }

    [Display(Name = "Trạng thái")]
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    public bool IsStockRestored { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    public ApplicationUser User { get; set; } = null!;
    public ICollection<OrderDetail> OrderDetails { get; set; } = [];
}
