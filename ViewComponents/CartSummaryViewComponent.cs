using Microsoft.AspNetCore.Mvc;
using MVC_nhaSach.Services;

namespace MVC_nhaSach.ViewComponents;

public class CartSummaryViewComponent(ICartService cartService) : ViewComponent
{
    public IViewComponentResult Invoke() => View(cartService.GetTotalQuantity());
}
