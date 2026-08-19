using Microsoft.AspNetCore.Mvc.Rendering;
using MVC_nhaSach.Models;

namespace MVC_nhaSach.ViewModels.Books;

public class BookSearchViewModel
{
    public string? Query { get; set; }
    public int? CategoryId { get; set; }
    public List<string> PriceRanges { get; set; } = [];
    public string Sort { get; set; } = "newest";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 8;
    public int TotalItems { get; set; }
    public IReadOnlyList<Book> Books { get; set; } = [];
    public IEnumerable<SelectListItem> Categories { get; set; } = [];
    public int TotalPages => TotalItems == 0 ? 0 : (int)Math.Ceiling(TotalItems / (double)PageSize);

    public static IReadOnlyList<PriceRangeOption> PriceOptions { get; } =
    [
        new("under_150", "Dưới 150.000 đ"),
        new("150_300", "150.000 đ – dưới 300.000 đ"),
        new("300_500", "300.000 đ – dưới 500.000 đ"),
        new("500_700", "500.000 đ – dưới 700.000 đ"),
        new("over_700", "Từ 700.000 đ trở lên")
    ];
}

public sealed record PriceRangeOption(string Value, string Label);
