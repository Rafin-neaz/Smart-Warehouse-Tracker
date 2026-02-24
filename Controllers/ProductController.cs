
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseTracker.Data;
using WarehouseTracker.Models;
using WarehouseTracker.Repositories;
using WarehouseTracker.Services;
using WarehouseTracker.ViewModels;

namespace WarehouseTracker.Controllers
{
    public class ProductController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IProductRepository _productRepo;
        private readonly IProductService _service;

        public ProductController(AppDbContext dbContext, IProductRepository productRepo, IProductService service)
        {
            _db = dbContext;
            _productRepo = productRepo;
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Index(ProductFilter filter)
        {
            var (products, hasMore) = await _service.GetProductsPagedAsync(filter);
            var categories = await _service.GetCategoriesAsync();

            ViewBag.Categories = categories;
            ViewData["HasMore"] = hasMore;


            var vm = new ProductListViewModel
            {
                Products = products,
                ActiveTab = filter.Tab,
            };
            if (Request.Headers.ContainsKey("HX-Request"))
            {
                return PartialView("_ProductRows", products);
            }
            var allProducts = await _productRepo.GetAll();
            var totalValue = allProducts.Sum(p => p.Price);

            ViewData["TotalInventoryValue"] = totalValue;
            return View("~/Views/Home/Index.cshtml", vm);

            
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
        public async Task<IActionResult> Create(Product product, decimal totalInventoryValue)
        {
            var categories = await _service.GetCategoriesAsync();

            ViewBag.Categories = categories;
            if (product.CategoryId != null && product.CategoryId.Value > 0)
            {
                ViewBag.SubCategories = await GetAllSubCategories(product.CategoryId.Value);
            }
            if (!ModelState.IsValid)
            {
                return PartialView("_AddNewForm", product);
            }

            product.CreatedAt = DateTime.UtcNow;
            product.UpdatedAt = DateTime.UtcNow;

            await _productRepo.CreateNew(product);
            ModelState.Clear();

            var totalValue = totalInventoryValue + product.Price.Value;
            Response.Headers.Add("HX-Trigger", @"{""showToast"": ""Product Created Successfully!"", ""inventory-updated"": {}}");
            return PartialView("_AddNewFormSuccess", (product, totalValue));
        }

        // GET: ProductController/Edit/5
        public async Task<ActionResult> EditRow(int id)
        {
            var query = _db.Products
                .Where(p => p.Id == id)
            .Include(p => p.Category)
            .Include(p => p.SubCategory)
            .AsQueryable();
            var product = await query.FirstOrDefaultAsync();
            return PartialView("_ProductEditRow", product);
        }


        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Update(Product product, decimal totalInventoryValue, decimal oldInventoryValue)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_ProductEditRow", product);
            }
            product.UpdatedAt = DateTime.UtcNow;
            await _productRepo.Update(product);
            decimal totalValue = totalInventoryValue - oldInventoryValue + product.Price.Value;
            Response.Headers.Add("HX-Trigger", @"{""showToast"": ""Product Updated Successfully!"", ""inventory-updated"": {}}");
            return PartialView("_ProductRowUpdated", (product, totalValue));
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, decimal totalInventoryValue, decimal deletedInventoryValue)
        {
            await _productRepo.Delete(id);
            var totalValue = totalInventoryValue - deletedInventoryValue;
            Response.Headers.Add("HX-Trigger", @"{""showToast"": ""Product Deleted Successfully!"", ""inventory-updated"": {}}");
            return PartialView("_TotalInventoryValue", totalValue);
        }

        [HttpGet]
        public async Task<IActionResult> SubCategories(int categoryId)
        {
            var subs = await _service.GetSubCategoriesAsync(categoryId);
            return PartialView("_SubCategoryOptions", subs);
        }
        public async Task<IEnumerable<SubCategory>> GetAllSubCategories(int categoryId)
        {
            var subs = await _service.GetSubCategoriesAsync(categoryId);
            return subs;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkUpdate([FromForm] BulkUpdateViewModel vm, ProductFilter filter)
        {
            if (vm.SelectedIds == null || !vm.SelectedIds.Any())
            {
                TempData["Error"] = "No products selected.";
                return PartialView("_BulkUpdateResult", new { Message = "No products selected.", Success = false });
            }

            var count = await _service.BulkUpdateStatusAsync(vm.SelectedIds, vm.NewStatus);

            var (products, hasMore) = await _service.GetProductsPagedAsync(filter);
            ViewData["HasMore"] = hasMore;
            Response.Headers.Add("HX-Trigger", @"{""bulk-update-success"": {}, ""inventory-updated"": {}}");
            return PartialView("_ProductRows", products);
        }
        [HttpGet]
        public async Task<IActionResult> Metrics()
        {
            var metrics = await _service.GetMetricsAsync();
            return PartialView("_Metrics", metrics);
        }

    }
}
