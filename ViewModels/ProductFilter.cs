namespace WarehouseTracker.ViewModels
{
    public class ProductFilter
    {
        public string Search { get; set; } = "";
        public string Tab { get; set; } = "all";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
