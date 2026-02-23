using Microsoft.AspNetCore.Http;
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
        // GET: ProductController
        [HttpGet]
        public async Task<IActionResult> Index(string search = "", string tab = "all", int page = 1)
        {
            tab = tab.ToLowerInvariant();
            var (products, hasMore) = await _service.GetProductsPagedAsync(search, tab, page);
            var categories = await _service.GetCategoriesAsync();

            ViewBag.Categories = categories;

            var totalValue = products.Sum(p => p.Price);

            ViewData["TotalInventoryValue"] = totalValue;
            //var metrics = await _service.GetMetricsAsync();
            //var categories = await _service.GetCategoriesAsync();

            ViewBag.Categories = categories;

            var vm = new ProductListViewModel
            {
                Products = products,
                ActiveTab = tab,
                Page = page,
                //HasMore = hasMore,
                //Metrics = metrics,
            };
            if (Request.Headers.ContainsKey("HX-Target"))
            {
                return PartialView("_ProductRows", products);
            }
            return View("~/Views/Home/Index.cshtml", vm);

            // If HTMX request for tab switch, return only the content partial
            //if (Request.IsHtmx() && Request.Headers.ContainsKey("HX-Target"))
            //{
            //    var target = Request.Headers["HX-Target"].ToString();
            //    if (target == "product-table-body")
            //    {
            //        return PartialView("_ProductTableBody", new InfiniteScrollViewModel
            //        {
            //            Products = products,
            //            NextPage = page + 1,
            //            HasMore = hasMore,
            //            ActiveTab = tab
            //        });
            //    }
            //}

            
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
            var categories = await _service.GetCategoriesAsync();

            ViewBag.Categories = categories;
            if (!ModelState.IsValid)
            {
                return PartialView("_AddNewForm", product);
            }

            product.CreatedAt = DateTime.UtcNow;
            product.UpdatedAt = DateTime.UtcNow;

            await _productRepo.CreateNew(product);
            ModelState.Clear();

            List<Product> products = await _productRepo.GetAll(null);
            var totalValue = products.Sum(p => p.Price) ?? 0;
            //Response.Headers["HX-Trigger"] = """{ "showToast": "Product Created Successfully!" }""";
            Response.Headers.Add("HX-Trigger", @"{""showToast"": ""Product Created Successfully!"", ""inventory-updated"": {}}");
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
            List<Product> products = await _productRepo.GetAll(null);
            decimal totalValue = products.Sum(p => p.Price) ?? 0;
            Response.Headers["HX-Trigger"] = """{ "showToast": "Product Updated Successfully!" }""";
            return PartialView("_ProductRowUpdated", (product, totalValue));
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            await _productRepo.Delete(id);
            List<Product> products = await _productRepo.GetAll(null);
            var totalValue = products.Sum(p => p.Price);
            Response.Headers["HX-Trigger"] = """{ "showToast": "Product Deleted Successfully!" }""";
            return PartialView("_TotalInventoryValue", totalValue);
        }

        [HttpGet]
        public async Task<IActionResult> SubCategories(int categoryId)
        {
            var subs = await _service.GetSubCategoriesAsync(categoryId);
            return PartialView("_SubCategoryOptions", subs);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkUpdate([FromForm] BulkUpdateViewModel vm)
        {
            if (vm.SelectedIds == null || !vm.SelectedIds.Any())
            {
                TempData["Error"] = "No products selected.";
                return PartialView("_BulkUpdateResult", new { Message = "No products selected.", Success = false });
            }

            var count = await _service.BulkUpdateStatusAsync(vm.SelectedIds, vm.NewStatus);

            //var (products, hasMore) = await _service.GetProductsPagedAsync("all", 1);
            //Response.Htmx(h =>
            //{
            //    h.WithTrigger("inventory-updated", timing: HtmxTriggerTiming.AfterSettle);
            //});

            // Return the updated rows for re-render
            //return PartialView("_ProductTableBody", new InfiniteScrollViewModel
            //{
            //    Products = products,
            //    NextPage = 2,
            //    HasMore = hasMore,
            //    ActiveTab = "all"
            //});
            List<Product> products = await _productRepo.GetAll(null);
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
