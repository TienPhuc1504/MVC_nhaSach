using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_nhaSach.Data;
using MVC_nhaSach.Models;

namespace MVC_nhaSach.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = IdentitySeeder.AdminRole)]
public class CategoriesController(ApplicationDbContext context) : Controller
{
    public async Task<IActionResult> Index()
    {
        var categories = await context.Categories
            .AsNoTracking()
            .Include(category => category.Books)
            .OrderBy(category => category.Name)
            .ToListAsync();
        return View(categories);
    }

    public async Task<IActionResult> Details(int id)
    {
        var category = await context.Categories
            .AsNoTracking()
            .Include(item => item.Books)
            .FirstOrDefaultAsync(item => item.Id == id);
        return category is null ? NotFound() : View(category);
    }

    [HttpGet]
    public IActionResult Create() => View(new Category());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Description")] Category category)
    {
        category.Name = (category.Name ?? string.Empty).Trim();
        if (await context.Categories.AnyAsync(item => item.Name == category.Name))
        {
            ModelState.AddModelError(nameof(Category.Name), "Tên danh mục đã tồn tại.");
        }

        if (!ModelState.IsValid)
        {
            return View(category);
        }

        context.Categories.Add(category);
        await context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Thêm danh mục thành công.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var category = await context.Categories.FindAsync(id);
        return category is null ? NotFound() : View(category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Category input)
    {
        if (id != input.Id)
        {
            return BadRequest();
        }

        input.Name = (input.Name ?? string.Empty).Trim();
        if (await context.Categories.AnyAsync(item => item.Id != id && item.Name == input.Name))
        {
            ModelState.AddModelError(nameof(Category.Name), "Tên danh mục đã tồn tại.");
        }

        if (!ModelState.IsValid)
        {
            return View(input);
        }

        var category = await context.Categories.FindAsync(id);
        if (category is null)
        {
            return NotFound();
        }

        category.Name = input.Name;
        category.Description = input.Description;
        await context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Cập nhật danh mục thành công.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await context.Categories
            .AsNoTracking()
            .Include(item => item.Books)
            .FirstOrDefaultAsync(item => item.Id == id);
        return category is null ? NotFound() : View(category);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var category = await context.Categories
            .Include(item => item.Books)
            .FirstOrDefaultAsync(item => item.Id == id);
        if (category is null)
        {
            return NotFound();
        }

        if (category.Books.Count > 0)
        {
            TempData["ErrorMessage"] = "Không thể xóa danh mục đang có sách.";
            return RedirectToAction(nameof(Index));
        }

        context.Categories.Remove(category);
        await context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Xóa danh mục thành công.";
        return RedirectToAction(nameof(Index));
    }
}
