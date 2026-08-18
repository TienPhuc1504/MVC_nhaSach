using Microsoft.AspNetCore.Identity;

namespace MVC_nhaSach.Models;

public class ApplicationUser : IdentityUser
{
    public ICollection<Order> Orders { get; set; } = [];
}
