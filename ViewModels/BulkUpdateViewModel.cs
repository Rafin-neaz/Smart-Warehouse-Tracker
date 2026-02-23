using WarehouseTracker.Enums;

namespace WarehouseTracker.ViewModels
{
    public class BulkUpdateViewModel
    {
        public List<int> SelectedIds { get; set; } = new();
        public ProductStatus NewStatus { get; set; }
    }
}
