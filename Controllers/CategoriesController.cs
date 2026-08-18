using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_nhaSach.Data;

namespace MVC_nhaSach.Controllers;

public class CategoriesController(ApplicationDbContext context) : Controller
{
    public async Task<IActionResult> Index() => View(await context.Categories.AsNoTracking()
        .Include(category => category.Books).OrderBy(category => category.Name).ToListAsync());
}
