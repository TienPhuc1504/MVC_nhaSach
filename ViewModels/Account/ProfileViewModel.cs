using System.ComponentModel.DataAnnotations;

namespace MVC_nhaSach.ViewModels.Account;

public class ProfileViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập họ và tên.")]
    [StringLength(100, ErrorMessage = "Họ và tên không được vượt quá 100 ký tự.")]
    [Display(Name = "Họ và tên")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
    [StringLength(20, MinimumLength = 8, ErrorMessage = "Số điện thoại phải có từ 8 đến 20 ký tự.")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
    [Display(Name = "Số điện thoại")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng.")]
    [StringLength(300, ErrorMessage = "Địa chỉ không được vượt quá 300 ký tự.")]
    [Display(Name = "Địa chỉ giao hàng")]
    public string Address { get; set; } = string.Empty;
}
