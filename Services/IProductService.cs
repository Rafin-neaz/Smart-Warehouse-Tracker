using WarehouseTracker.Enums;
using WarehouseTracker.Models;
using WarehouseTracker.ViewModels;

namespace WarehouseTracker.Services
{
    public interface IProductService
    {
        Task<(IEnumerable<Product> Products, bool HasMore)> GetProductsPagedAsync(string search, string tab, int page, int pageSize = 20);
        Task<Product?> GetByIdAsync(int id);
        //Task<Product> CreateAsync(ProductCreateViewModel vm);
        //Task<Product> UpdateAsync(int id, ProductEditViewModel vm);
        Task DeleteAsync(int id);
        Task<int> BulkUpdateStatusAsync(IEnumerable<int> ids, ProductStatus newStatus);
        Task<DashboardMetrics> GetMetricsAsync();
        Task<IEnumerable<SubCategory>> GetSubCategoriesAsync(int categoryId);
        Task<IEnumerable<Category>> GetCategoriesAsync();
    }
}
