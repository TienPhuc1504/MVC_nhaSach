using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC_nhaSach.Areas.Admin.ViewModels;
using MVC_nhaSach.Data;
using MVC_nhaSach.Models;
using MVC_nhaSach.Services;

namespace MVC_nhaSach.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = IdentitySeeder.AdminRole)]
public class BooksController(ApplicationDbContext context, IImageService imageService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var books = await context.Books.AsNoTracking()
            .Include(book => book.Category)
            .OrderByDescending(book => book.CreatedDate)
            .ToListAsync();
        return View(books);
    }

    public async Task<IActionResult> Details(int id)
    {
        var book = await context.Books.AsNoTracking()
            .Include(item => item.Category)
            .FirstOrDefaultAsync(item => item.Id == id);
        return book is null ? NotFound() : View(book);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new BookFormViewModel();
        await PopulateCategoriesAsync(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(ImageService.MaxFileSize + 100_000)]
    public async Task<IActionResult> Create(BookFormViewModel model, CancellationToken cancellationToken)
    {
        await ValidateCategoryAsync(model.CategoryId);
        if (!ModelState.IsValid)
        {
            await PopulateCategoriesAsync(model);
            return View(model);
        }

        string? imagePath = null;
        try
        {
            if (model.ImageFile is not null)
            {
                imagePath = await imageService.SaveBookImageAsync(model.ImageFile, cancellationToken);
            }

            var book = new Book
            {
                Title = model.Title.Trim(),
                Author = model.Author.Trim(),
                Price = model.Price,
                StockQuantity = model.StockQuantity,
                Description = model.Description,
                ImagePath = imagePath,
                IsFeatured = model.IsFeatured,
                CategoryId = model.CategoryId,
                CreatedDate = DateTime.UtcNow
            };
            context.Books.Add(book);
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (InvalidOperationException exception)
        {
            ModelState.AddModelError(nameof(model.ImageFile), exception.Message);
            await PopulateCategoriesAsync(model);
            return View(model);
        }
        catch
        {
            imageService.DeleteBookImage(imagePath);
            throw;
        }

        TempData["SuccessMessage"] = "Thêm sách thành công.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var book = await context.Books.AsNoTracking().FirstOrDefaultAsync(item => item.Id == id);
        if (book is null)
        {
            return NotFound();
        }

        var model = new BookFormViewModel
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            Price = book.Price,
            StockQuantity = book.StockQuantity,
            Description = book.Description,
            IsFeatured = book.IsFeatured,
            CategoryId = book.CategoryId,
            CurrentImagePath = book.ImagePath
        };
        await PopulateCategoriesAsync(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(ImageService.MaxFileSize + 100_000)]
    public async Task<IActionResult> Edit(int id, BookFormViewModel model, CancellationToken cancellationToken)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        var book = await context.Books.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (book is null)
        {
            return NotFound();
        }

        model.CurrentImagePath = book.ImagePath;
        await ValidateCategoryAsync(model.CategoryId);
        if (!ModelState.IsValid)
        {
            await PopulateCategoriesAsync(model);
            return View(model);
        }

        string? newImagePath = null;
        try
        {
            if (model.ImageFile is not null)
            {
                newImagePath = await imageService.SaveBookImageAsync(model.ImageFile, cancellationToken);
            }

            var oldImagePath = book.ImagePath;
            book.Title = model.Title.Trim();
            book.Author = model.Author.Trim();
            book.Price = model.Price;
            book.StockQuantity = model.StockQuantity;
            book.Description = model.Description;
            book.IsFeatured = model.IsFeatured;
            book.CategoryId = model.CategoryId;
            if (newImagePath is not null)
            {
                book.ImagePath = newImagePath;
            }

            await context.SaveChangesAsync(cancellationToken);
            if (newImagePath is not null)
            {
                imageService.DeleteBookImage(oldImagePath);
            }
        }
        catch (InvalidOperationException exception)
        {
            ModelState.AddModelError(nameof(model.ImageFile), exception.Message);
            await PopulateCategoriesAsync(model);
            return View(model);
        }
        catch
        {
            imageService.DeleteBookImage(newImagePath);
            throw;
        }

        TempData["SuccessMessage"] = "Cập nhật sách thành công.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var book = await context.Books.AsNoTracking()
            .Include(item => item.Category)
            .Include(item => item.OrderDetails)
            .FirstOrDefaultAsync(item => item.Id == id);
        return book is null ? NotFound() : View(book);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var book = await context.Books
            .Include(item => item.OrderDetails)
            .FirstOrDefaultAsync(item => item.Id == id);
        if (book is null)
        {
            return NotFound();
        }

        if (book.OrderDetails.Count > 0)
        {
            TempData["ErrorMessage"] = "Không thể xóa sách đã xuất hiện trong đơn hàng.";
            return RedirectToAction(nameof(Index));
        }

        var imagePath = book.ImagePath;
        context.Books.Remove(book);
        await context.SaveChangesAsync();
        imageService.DeleteBookImage(imagePath);
        TempData["SuccessMessage"] = "Xóa sách thành công.";
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateCategoriesAsync(BookFormViewModel model)
    {
        model.Categories = await context.Categories.AsNoTracking()
            .OrderBy(category => category.Name)
            .Select(category => new SelectListItem(category.Name, category.Id.ToString()))
            .ToListAsync();
    }

    private async Task ValidateCategoryAsync(int categoryId)
    {
        if (categoryId > 0 && !await context.Categories.AnyAsync(category => category.Id == categoryId))
        {
            ModelState.AddModelError(nameof(BookFormViewModel.CategoryId), "Danh mục không tồn tại.");
        }
    }
}
