using Microsoft.AspNetCore.Mvc;
using WarehouseTracker.Data;
using WarehouseTracker.Models;
using WarehouseTracker.Repositories;
using WarehouseTracker.Services;
using WarehouseTracker.ViewModels;

namespace WarehouseTracker.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductRepository _repo;
        private readonly IProductService _service;

        public HomeController(IProductRepository productRepository, IProductService service)
        {
            _repo = productRepository;
            _service = service;
        }
        public async Task<IActionResult> Index(string? search)
        {
            return RedirectToAction("Index", "Product", new { search });
        }
    }
}
