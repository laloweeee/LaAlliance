using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ASI.Basecode.Services.ServiceModels
{
    public class ProductViewModel
    {
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [StringLength(100, ErrorMessage = "Product name cannot exceed 100 characters")]
        public string ProductName { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string ProductDescription { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal ProductPrice { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public int CategoryID { get; set; }
        public bool IsActive { get; set; } = true;

        [Display(Name = "Product Image")]
        public IFormFile ImageFile { get; set; }
        public string ProductImage { get; set; }
        public List<ProductOptionGroupViewModel> CustomizationGroups { get; set; } = new List<ProductOptionGroupViewModel>();
    }
}