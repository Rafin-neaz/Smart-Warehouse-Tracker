using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WarehouseTracker.Enums;

namespace WarehouseTracker.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Proudct Name is required")]
        [StringLength(125, MinimumLength = 1, ErrorMessage = "Proudct name must be between 2 and 125 characters")]
        public string Name { get; set; }
        [StringLength(250, ErrorMessage = "Proudct description can take upto 250 characters")]
        public string? Description { get; set; }
        [Required(ErrorMessage = "Unit price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than zero.")]
        public decimal? Price { get; set; } = 0;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        [Required(ErrorMessage = "Proudct category is required")]
        public int? CategoryId { get; set; }
        
        public Category? Category { get; set; } = null!;
        [Required(ErrorMessage = "Proudct sub category is required")]
        public int? SubCategoryId { get; set; }
        
        public SubCategory? SubCategory { get; set; }
        public int StockQuantity { get; set; }

        public ProductStatus Status { get; set; } = ProductStatus.InStock;
        // Concurrency token
        [Timestamp]
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();

        public bool IsLowStock => StockQuantity < 10;
        public bool IsOutOfStock => StockQuantity == 0 || Status == ProductStatus.OutOfStock;

    }
}
