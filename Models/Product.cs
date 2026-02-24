using System.ComponentModel.DataAnnotations;
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
        [Display(Name = "Per Unit Price")]
        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        public decimal? Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        [Required(ErrorMessage = "Proudct category is required")]
        public int? CategoryId { get; set; }
        
        public Category? Category { get; set; } = null!;
        [Required(ErrorMessage = "Proudct sub category is required")]
        public int? SubCategoryId { get; set; }
        
        public SubCategory? SubCategory { get; set; }
        [Required(ErrorMessage = "Stock Quantity of the Product is required")]
        public int? StockQuantity { get; set; }

        public ProductStatus Status { get; set; } = ProductStatus.InStock;

        [Timestamp]
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();

        public bool IsLowStock => StockQuantity < 10;
        public bool IsOutOfStock => StockQuantity == 0 || Status == ProductStatus.OutOfStock;

    }
}
