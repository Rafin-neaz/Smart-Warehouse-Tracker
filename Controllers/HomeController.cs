using Microsoft.AspNetCore.Mvc;
using WarehouseTracker.Data;
using WarehouseTracker.Models;
using WarehouseTracker.Repositories;

namespace WarehouseTracker.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductRepository _repo;

        public HomeController(IProductRepository productRepository)
        {
            _repo = productRepository;
        }
        public async Task<IActionResult> Index(string? search)
        {
            List<Product> products = await _repo.GetAll();
            if (!string.IsNullOrWhiteSpace(search))
            {
                products = products.Where(prod => prod.Name.Contains(search)).ToList();
            }

            var totalValue = products.Sum(p => p.Price);

            ViewData["TotalInventoryValue"] = totalValue;

            if (Request.Headers.ContainsKey("HX-Request"))
            {
                return PartialView("_ProductRows", products);
            }
            
            return View(products);
        }
    }
}
