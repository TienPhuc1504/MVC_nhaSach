using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_nhaSach.Data;
using MVC_nhaSach.Models;
using MVC_nhaSach.ViewModels.Home;
using System.Diagnostics;

namespace MVC_nhaSach.Controllers
{
    public class HomeController(ApplicationDbContext context) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var model = new HomeViewModel
            {
                LatestBooks = await context.Books.AsNoTracking().Include(book => book.Category)
                    .OrderByDescending(book => book.CreatedDate).Take(8).ToListAsync(),
                FeaturedBooks = await context.Books.AsNoTracking().Include(book => book.Category)
                    .Where(book => book.IsFeatured).OrderByDescending(book => book.CreatedDate).Take(8).ToListAsync()
            };
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
