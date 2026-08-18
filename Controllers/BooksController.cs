using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC_nhaSach.Data;
using MVC_nhaSach.ViewModels.Books;

namespace MVC_nhaSach.Controllers;

public class BooksController(ApplicationDbContext context) : Controller
{
    public async Task<IActionResult> Index(BookSearchViewModel model)
    {
        model.Page = Math.Max(1, model.Page);
        model.PageSize = 8;
        if (model.MinPrice < 0) ModelState.AddModelError(nameof(model.MinPrice), "Giá tối thiểu không được âm.");
        if (model.MaxPrice < 0) ModelState.AddModelError(nameof(model.MaxPrice), "Giá tối đa không được âm.");
        if (model.MinPrice.HasValue && model.MaxPrice.HasValue && model.MinPrice > model.MaxPrice)
            ModelState.AddModelError(string.Empty, "Giá tối thiểu không được lớn hơn giá tối đa.");
        var query = context.Books.AsNoTracking().Include(book => book.Category).AsQueryable();
        if (!string.IsNullOrWhiteSpace(model.Title)) { var value = model.Title.Trim(); query = query.Where(book => book.Title.Contains(value)); }
        if (!string.IsNullOrWhiteSpace(model.Author)) { var value = model.Author.Trim(); query = query.Where(book => book.Author.Contains(value)); }
        if (model.CategoryId.HasValue) query = query.Where(book => book.CategoryId == model.CategoryId.Value);
        if (model.MinPrice.HasValue) query = query.Where(book => book.Price >= model.MinPrice.Value);
        if (model.MaxPrice.HasValue) query = query.Where(book => book.Price <= model.MaxPrice.Value);
        query = model.Sort switch
        {
            "price_asc" => query.OrderBy(book => book.Price).ThenBy(book => book.Title),
            "price_desc" => query.OrderByDescending(book => book.Price).ThenBy(book => book.Title),
            _ => query.OrderByDescending(book => book.CreatedDate)
        };
        model.TotalItems = await query.CountAsync();
        if (model.TotalPages > 0 && model.Page > model.TotalPages) model.Page = model.TotalPages;
        model.Books = await query.Skip((model.Page - 1) * model.PageSize).Take(model.PageSize).ToListAsync();
        model.Categories = await context.Categories.AsNoTracking().OrderBy(category => category.Name)
            .Select(category => new SelectListItem(category.Name, category.Id.ToString())).ToListAsync();
        return View(model);
    }

    public async Task<IActionResult> Details(int id)
    {
        var book = await context.Books.AsNoTracking().Include(item => item.Category).FirstOrDefaultAsync(item => item.Id == id);
        return book is null ? NotFound() : View(book);
    }
}
