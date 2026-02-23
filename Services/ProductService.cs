using Microsoft.EntityFrameworkCore;
using WarehouseTracker.Data;
using WarehouseTracker.Enums;
using WarehouseTracker.Models;
using WarehouseTracker.ViewModels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WarehouseTracker.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _db;

        public ProductService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<(IEnumerable<Product> Products, bool HasMore)> GetProductsPagedAsync(string search, string tab, int page, int pageSize = 20)
        {
            var query = _db.Products
            .Include(p => p.Category)
            .Include(p => p.SubCategory)
            .AsQueryable();

            // Apply search filter if provided
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p => p.Name.Contains(search));
            }

            // Apply tab filter
            query = tab.ToLowerInvariant() switch
            {
                "incoming" => query.Where(p => p.Status == ProductStatus.InTransit),
                "outofstock" => query.Where(p => p.Status == ProductStatus.OutOfStock || p.StockQuantity == 0),
                _ => query
            };

            query = query.OrderByDescending(p => p.UpdatedAt);

            //var skip = (page - 1) * pageSize;
            //var items = await query.Skip(skip).Take(pageSize + 1).ToListAsync();
            //var hasMore = items.Count > pageSize;
            //if (hasMore) items = items.Take(pageSize).ToList();

            //return (items, hasMore);
            var items = await query.ToListAsync();
            return (items, false);
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _db.Products
                .Include(p => p.Category)
                .Include(p => p.SubCategory)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        //public async Task<Product> CreateAsync(ProductCreateViewModel vm)
        //{
        //    var product = new Product
        //    {
        //        Name = vm.Name,
        //        Sku = vm.Sku,
        //        Price = vm.Price,
        //        StockQuantity = vm.StockQuantity,
        //        Status = vm.Status,
        //        CategoryId = vm.CategoryId,
        //        SubCategoryId = vm.SubCategoryId,
        //        Origin = vm.Origin,
        //        Destination = vm.Destination,
        //        CreatedAt = DateTime.UtcNow,
        //        UpdatedAt = DateTime.UtcNow,
        //    };
        //    _db.Products.Add(product);
        //    await _db.SaveChangesAsync();
        //    await _db.Entry(product).Reference(p => p.Category).LoadAsync();
        //    if (product.SubCategoryId.HasValue)
        //        await _db.Entry(product).Reference(p => p.SubCategory).LoadAsync();
        //    return product;
        //}

        //public async Task<Product> UpdateAsync(int id, ProductEditViewModel vm)
        //{
        //    var product = await _db.Products.FindAsync(id)
        //        ?? throw new KeyNotFoundException($"Product {id} not found");

        //    // Concurrency check
        //    if (vm.RowVersion != null)
        //    {
        //        var incoming = Convert.FromBase64String(vm.RowVersion);
        //        if (!product.RowVersion.SequenceEqual(incoming))
        //            throw new DbUpdateConcurrencyException("This record was modified by another user. Please reload and try again.");
        //    }

        //    product.Name = vm.Name;
        //    product.Sku = vm.Sku;
        //    product.Price = vm.Price;
        //    product.StockQuantity = vm.StockQuantity;
        //    product.Status = vm.Status;
        //    product.CategoryId = vm.CategoryId;
        //    product.SubCategoryId = vm.SubCategoryId;
        //    product.Origin = vm.Origin;
        //    product.Destination = vm.Destination;
        //    product.UpdatedAt = DateTime.UtcNow;

        //    await _db.SaveChangesAsync();
        //    await _db.Entry(product).Reference(p => p.Category).LoadAsync();
        //    if (product.SubCategoryId.HasValue)
        //        await _db.Entry(product).Reference(p => p.SubCategory).LoadAsync();
        //    return product;
        //}

        public async Task DeleteAsync(int id)
        {
            var product = await _db.Products.FindAsync(id)
                ?? throw new KeyNotFoundException($"Product {id} not found");
            _db.Products.Remove(product);
            await _db.SaveChangesAsync();
        }

        public async Task<int> BulkUpdateStatusAsync(IEnumerable<int> ids, ProductStatus newStatus)
        {
            var idList = ids.ToList();
            var products = await _db.Products
                .Where(p => idList.Contains(p.Id))
                .ToListAsync();

            foreach (var p in products)
            {
                p.Status = newStatus;
                p.UpdatedAt = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();
            return products.Count;
        }

        public async Task<DashboardMetrics> GetMetricsAsync()
        {
            var lowStockItems = await _db.Products
                .Where(p => p.StockQuantity > 0 && p.StockQuantity < 10)
                .OrderBy(p => p.StockQuantity)
                .Take(10)
                .ToListAsync();

            var activeShipmentItems = await _db.Products
                .Where(p => p.Status == ProductStatus.InTransit)
                .OrderByDescending(p => p.UpdatedAt)
                .Take(10)
                .ToListAsync();

            return new DashboardMetrics
            {
                TotalProducts = await _db.Products.CountAsync(),
                ActiveShipments = await _db.Products.CountAsync(p => p.Status == ProductStatus.InTransit),
                LowStockCount = await _db.Products.CountAsync(p => p.StockQuantity > 0 && p.StockQuantity < 10),
                OutOfStockCount = await _db.Products.CountAsync(p => p.StockQuantity == 0 || p.Status == ProductStatus.OutOfStock),
                DelayedShipments = await _db.Products.CountAsync(p => p.Status == ProductStatus.Delayed),
                LowStockItems = lowStockItems,
                ActiveShipmentItems = activeShipmentItems,
            };
        }

        public async Task<IEnumerable<SubCategory>> GetSubCategoriesAsync(int categoryId)
        {
            return await _db.SubCategories
                .Where(sc => sc.CategoryId == categoryId)
                .OrderBy(sc => sc.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return await _db.Categories.OrderBy(c => c.Name).ToListAsync();
        }
    }

}
