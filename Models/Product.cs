using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WarehouseTracker.Enums;

namespace WarehouseTracker.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Proudct Name is required")]
        [StringLength(25, MinimumLength = 2, ErrorMessage = "Proudct name must be between 2 and 25 characters")]
        public string Name { get; set; }
        [StringLength(250, ErrorMessage = "Proudct description can take upto 250 characters")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Unit price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than zero.")]
        [Display(Name = "Per Unit Price")]
        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        public decimal? Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        [Required(ErrorMessage = "Proudct category is required")]
        public ProductCategory? Category { get; set; }
        
    }
}
