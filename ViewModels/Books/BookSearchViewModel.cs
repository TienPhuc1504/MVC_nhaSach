using Microsoft.AspNetCore.Mvc.Rendering;
using MVC_nhaSach.Models;

namespace MVC_nhaSach.ViewModels.Books;

public class BookSearchViewModel
{
    public string? Title { get; set; }
    public string? Author { get; set; }
    public int? CategoryId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string Sort { get; set; } = "newest";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 8;
    public int TotalItems { get; set; }
    public IReadOnlyList<Book> Books { get; set; } = [];
    public IEnumerable<SelectListItem> Categories { get; set; } = [];
    public int TotalPages => TotalItems == 0 ? 0 : (int)Math.Ceiling(TotalItems / (double)PageSize);
}
