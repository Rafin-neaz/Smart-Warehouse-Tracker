using WarehouseTracker.Enums;
using WarehouseTracker.Models;
using WarehouseTracker.ViewModels;

namespace WarehouseTracker.Services
{
    public interface IProductService
    {
        Task<(IEnumerable<Product> Products, bool HasMore)> GetProductsPagedAsync(ProductFilter filter);
        Task<int> BulkUpdateStatusAsync(IEnumerable<int> ids, ProductStatus newStatus);
        Task<DashboardMetrics> GetMetricsAsync();
        Task<IEnumerable<SubCategory>> GetSubCategoriesAsync(int categoryId);
        Task<IEnumerable<Category>> GetCategoriesAsync();
    }
}
