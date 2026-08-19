using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC_nhaSach.Data;
using MVC_nhaSach.ViewModels.Books;

namespace MVC_nhaSach.Controllers;

public class BooksController(ApplicationDbContext context) : Controller
{
    private const string AccentInsensitiveSearchCollation = "Latin1_General_100_CI_AI";

    public async Task<IActionResult> Index(BookSearchViewModel model)
    {
        model.Page = Math.Max(1, model.Page);
        model.PageSize = 9;
        var validPriceRanges = BookSearchViewModel.PriceOptions
            .Select(option => option.Value)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        model.PriceRanges = model.PriceRanges
            .Where(validPriceRanges.Contains)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var booksQuery = context.Books.AsNoTracking().Include(book => book.Category).AsQueryable();
        if (!string.IsNullOrWhiteSpace(model.Query))
        {
            var value = model.Query.Trim();
            booksQuery = booksQuery.Where(book =>
                EF.Functions.Collate(book.Title, AccentInsensitiveSearchCollation).Contains(value)
                || EF.Functions.Collate(book.Author, AccentInsensitiveSearchCollation).Contains(value));
        }

        if (model.CategoryId.HasValue)
        {
            booksQuery = booksQuery.Where(book => book.CategoryId == model.CategoryId.Value);
        }

        if (model.PriceRanges.Count > 0)
        {
            var under150 = model.PriceRanges.Contains("under_150", StringComparer.OrdinalIgnoreCase);
            var from150To300 = model.PriceRanges.Contains("150_300", StringComparer.OrdinalIgnoreCase);
            var from300To500 = model.PriceRanges.Contains("300_500", StringComparer.OrdinalIgnoreCase);
            var from500To700 = model.PriceRanges.Contains("500_700", StringComparer.OrdinalIgnoreCase);
            var over700 = model.PriceRanges.Contains("over_700", StringComparer.OrdinalIgnoreCase);
            booksQuery = booksQuery.Where(book =>
                (under150 && book.Price < 150000)
                || (from150To300 && book.Price >= 150000 && book.Price < 300000)
                || (from300To500 && book.Price >= 300000 && book.Price < 500000)
                || (from500To700 && book.Price >= 500000 && book.Price < 700000)
                || (over700 && book.Price >= 700000));
        }

        booksQuery = model.Sort switch
        {
            "price_asc" => booksQuery.OrderBy(book => book.Price).ThenBy(book => book.Title),
            "price_desc" => booksQuery.OrderByDescending(book => book.Price).ThenBy(book => book.Title),
            "title_asc" => booksQuery.OrderBy(book => book.Title),
            _ => booksQuery.OrderByDescending(book => book.CreatedDate)
        };
        model.TotalItems = await booksQuery.CountAsync();
        if (model.TotalPages > 0 && model.Page > model.TotalPages) model.Page = model.TotalPages;
        model.Books = await booksQuery.Skip((model.Page - 1) * model.PageSize).Take(model.PageSize).ToListAsync();
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
