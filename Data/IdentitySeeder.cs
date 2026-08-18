using Microsoft.AspNetCore.Identity;
using MVC_nhaSach.Models;

namespace MVC_nhaSach.Data;

public static class IdentitySeeder
{
    public const string AdminRole = "Admin";
    public const string CustomerRole = "Customer";

    public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration)
    {
        await using var scope = services.CreateAsyncScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
            .CreateLogger(nameof(IdentitySeeder));

        foreach (var roleName in new[] { AdminRole, CustomerRole })
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                EnsureSucceeded(roleResult, $"Không thể tạo role {roleName}.");
            }
        }

        // Chuẩn hóa tài khoản Customer từng được tạo bởi smoke test cũ.
        var users = userManager.Users.ToList();
        var testCustomer = users.FirstOrDefault(user =>
            user.Email?.StartsWith("customer.", StringComparison.OrdinalIgnoreCase) == true &&
            user.Email.EndsWith("@example.local", StringComparison.OrdinalIgnoreCase));
        if (testCustomer is not null && await userManager.FindByEmailAsync("customer@nhasach.local") is null)
        {
            EnsureSucceeded(await userManager.SetEmailAsync(testCustomer, "customer@nhasach.local"),
                "Không thể chuẩn hóa email Customer demo.");
            EnsureSucceeded(await userManager.SetUserNameAsync(testCustomer, "customer@nhasach.local"),
                "Không thể chuẩn hóa tên đăng nhập Customer demo.");
        }

        await SeedUserAsync(
            userManager,
            configuration["AdminSeed:Email"],
            configuration["AdminSeed:Password"],
            AdminRole,
            logger);

        await SeedUserAsync(
            userManager,
            configuration["CustomerSeed:Email"],
            configuration["CustomerSeed:Password"],
            CustomerRole,
            logger);
    }

    private static async Task SeedUserAsync(
        UserManager<ApplicationUser> userManager,
        string? email,
        string? password,
        string role,
        ILogger logger)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            logger.LogWarning(
                "Chưa cấu hình đầy đủ tài khoản {Role} demo. Xem README.md để thiết lập bằng User Secrets.",
                role);
            return;
        }

        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(user, password);
            EnsureSucceeded(createResult, $"Không thể tạo tài khoản {role} demo.");
        }

        if (!await userManager.IsInRoleAsync(user, role))
        {
            var addRoleResult = await userManager.AddToRoleAsync(user, role);
            EnsureSucceeded(addRoleResult, $"Không thể gán role {role}.");
        }
    }

    private static void EnsureSucceeded(IdentityResult result, string message)
    {
        if (result.Succeeded)
        {
            return;
        }

        var errors = string.Join("; ", result.Errors.Select(error => error.Description));
        throw new InvalidOperationException($"{message} {errors}");
    }
}
