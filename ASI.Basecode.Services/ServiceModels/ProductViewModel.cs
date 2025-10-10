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
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public int CategoryID { get; set; }
        public bool IsActive { get; set; } = true;

        [Display(Name = "Product Image")]
        public IFormFile ImageFile { get; set; }
        public string ImageUrl { get; set; }
        public List<CustomizationGroupViewModel> CustomizationGroups { get; set; } = [];
    }
}