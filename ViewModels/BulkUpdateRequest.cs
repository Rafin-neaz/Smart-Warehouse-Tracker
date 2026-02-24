using WarehouseTracker.Enums;

namespace WarehouseTracker.ViewModels
{
    public class BulkUpdateRequest
    {
        public List<int> SelectedIds { get; set; } = new();
        public ProductStatus NewStatus { get; set; }
        public ProductFilter Filter { get; set; } = new();
    }
}
