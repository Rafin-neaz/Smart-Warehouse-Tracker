using WarehouseTracker.Models;

namespace WarehouseTracker.ViewModels
{
    public class ProductListViewModel
    {
        public IEnumerable<Product> Products { get; set; } = Enumerable.Empty<Product>();
        public string ActiveTab { get; set; } = "all";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public DashboardMetrics Metrics { get; set; } = new();
    }
}
