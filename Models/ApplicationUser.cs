using Microsoft.AspNetCore.Identity;

namespace MVC_nhaSach.Models;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;

    public string ShippingAddress { get; set; } = string.Empty;

    public ICollection<Order> Orders { get; set; } = [];
}
