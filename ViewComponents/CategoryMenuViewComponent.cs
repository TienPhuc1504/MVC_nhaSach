using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_nhaSach.Data;

namespace MVC_nhaSach.ViewComponents;

public class CategoryMenuViewComponent(ApplicationDbContext context) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync() => View(await context.Categories.AsNoTracking()
        .OrderBy(category => category.Name).ToListAsync());
}
