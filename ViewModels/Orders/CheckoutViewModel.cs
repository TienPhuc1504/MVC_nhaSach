using System.ComponentModel.DataAnnotations;

namespace MVC_nhaSach.ViewModels.Orders;

public class CheckoutViewModel : IValidatableObject
{
    [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự.")]
    [Display(Name = "Họ tên người nhận")]
    public string CustomerName { get; set; } = string.Empty;

    [StringLength(20, MinimumLength = 8, ErrorMessage = "Số điện thoại phải có từ 8 đến 20 ký tự.")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
    [Display(Name = "Số điện thoại")]
    public string Phone { get; set; } = string.Empty;

    [StringLength(300, ErrorMessage = "Địa chỉ không được vượt quá 300 ký tự.")]
    [Display(Name = "Địa chỉ giao hàng")]
    public string Address { get; set; } = string.Empty;

    [Display(Name = "Đặt cho")]
    public bool DeliverToMyself { get; set; }

    [StringLength(500, ErrorMessage = "Ghi chú không được vượt quá 500 ký tự.")]
    [Display(Name = "Ghi chú đơn hàng")]
    public string? Note { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(CustomerName))
            yield return new ValidationResult("Vui lòng nhập họ tên người nhận.", [nameof(CustomerName)]);
        if (string.IsNullOrWhiteSpace(Phone))
            yield return new ValidationResult("Vui lòng nhập số điện thoại.", [nameof(Phone)]);
        if (string.IsNullOrWhiteSpace(Address))
            yield return new ValidationResult("Vui lòng nhập địa chỉ giao hàng.", [nameof(Address)]);
    }
}
