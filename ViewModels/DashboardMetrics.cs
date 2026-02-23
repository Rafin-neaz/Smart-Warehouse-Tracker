using WarehouseTracker.Models;

namespace WarehouseTracker.ViewModels
{
    public class DashboardMetrics
    {
        public int TotalProducts { get; set; }
        public int ActiveShipments { get; set; }
        public int LowStockCount { get; set; }
        public int OutOfStockCount { get; set; }
        public int DelayedShipments { get; set; }
        public IEnumerable<Product> LowStockItems { get; set; } = Enumerable.Empty<Product>();
        public IEnumerable<Product> ActiveShipmentItems { get; set; } = Enumerable.Empty<Product>();
    }
}
