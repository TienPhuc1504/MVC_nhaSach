using MVC_nhaSach.Models;

namespace MVC_nhaSach.ViewModels.Home;

public class HomeViewModel
{
    public IReadOnlyList<Book> LatestBooks { get; set; } = [];
    public IReadOnlyList<Book> FeaturedBooks { get; set; } = [];
}
