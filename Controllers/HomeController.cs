using Microsoft.AspNetCore.Mvc;

namespace WarehouseTracker.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index(string? search)
        {
            return RedirectToAction("Index", "Product", new { search });
        }
    }
}
