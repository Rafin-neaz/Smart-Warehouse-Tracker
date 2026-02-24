using Microsoft.EntityFrameworkCore;
using WarehouseTracker.Data;
using WarehouseTracker.Enums;
using WarehouseTracker.Models;
using WarehouseTracker.ViewModels;

namespace WarehouseTracker.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _db;

        public ProductService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<(IEnumerable<Product> Products, bool HasMore)> GetProductsPagedAsync(ProductFilter filter)
        {
            var query = _db.Products
            .Include(p => p.Category)
            .Include(p => p.SubCategory)
            .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                query = query.Where(p => p.Name.Contains(filter.Search));
            }

            query = filter.Tab.ToLowerInvariant() switch
            {
                "incoming" => query.Where(p => p.Status == ProductStatus.InTransit),
                "outofstock" => query.Where(p => p.Status == ProductStatus.OutOfStock || p.StockQuantity == 0),
                _ => query
            };

            query = query.OrderByDescending(p => p.UpdatedAt);

            var skip = (filter.Page - 1) * filter.PageSize;
            var items = await query.Skip(skip).Take(filter.PageSize + 1).ToListAsync();
            var hasMore = items.Count > filter.PageSize;
            if (hasMore) items = items.Take(filter.PageSize).ToList();

            return (items, hasMore);
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
