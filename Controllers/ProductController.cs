using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseTracker.Data;
using WarehouseTracker.Models;
using WarehouseTracker.Repositories;

namespace WarehouseTracker.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IProductRepository _productRepo;

        public ProductController(AppDbContext dbContext, IProductRepository productRepo)
        {
            _db = dbContext;
            _productRepo = productRepo;
        }
        // GET: ProductController
        public ActionResult Index()
        {
            return View();
        }

        // GET: ProductController/Details/5
        public async Task<ActionResult> ReadRow(int id)
        {
            var product = await _productRepo.Get(id);
            return PartialView("_ProductRow", product);
        }

        // GET: ProductController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProductController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_AddNewForm", product);
            }

            product.CreatedAt = DateTime.UtcNow;
            product.UpdatedAt = DateTime.UtcNow;

            await _productRepo.CreateNew(product);
            ModelState.Clear();

            List<Product> products = await _productRepo.GetAll();
            var totalValue = products.Sum(p => p.Price) ?? 0;
            Response.Headers["HX-Trigger"] = """{ "showToast": "Product Created Successfully!" }""";
            return PartialView("_AddNewFormSuccess", (product, totalValue));
        }

        // GET: ProductController/Edit/5
        public async Task<ActionResult> EditRow(int id)
        {
            var product = await _productRepo.Get(id);
            return PartialView("_ProductEditRow", product);
        }


        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Update(Product product)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_ProductEditRow", product);
            }
            await _productRepo.Update(product);
            List<Product> products = await _productRepo.GetAll();
            decimal totalValue = products.Sum(p => p.Price) ?? 0;
            Response.Headers["HX-Trigger"] = """{ "showToast": "Product Updated Successfully!" }""";
            return PartialView("_ProductRowUpdated", (product, totalValue));
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            await _productRepo.Delete(id);
            List<Product> products = await _productRepo.GetAll();
            var totalValue = products.Sum(p => p.Price);
            Response.Headers["HX-Trigger"] = """{ "showToast": "Product Deleted Successfully!" }""";
            return PartialView("_TotalInventoryValue", totalValue);
        }
    }
}
