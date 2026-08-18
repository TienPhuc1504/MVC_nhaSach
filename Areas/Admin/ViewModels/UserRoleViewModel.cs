namespace MVC_nhaSach.Areas.Admin.ViewModels;

public class UserRoleViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public IReadOnlyList<string> Roles { get; set; } = [];
    public bool IsAdmin => Roles.Contains("Admin");
}
