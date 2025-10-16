using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ASI.Basecode.Services.ServiceModels
{
    /// <summary>
    /// ViewModel for products, including validation attributes and nested customization groups.
    /// </summary>
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
        public string CategoryName { get; set; }
        public bool IsActive { get; set; } = true;
        [Display(Name = "Product Image")]
        public IFormFile ImageFile { get; set; }
        public string ProductImage { get; set; }
        public List<ProductOptionGroupViewModel> CustomizationGroups { get; set; } = new List<ProductOptionGroupViewModel>();
    }

    /// <summary>
    /// ViewModel for product option groups, including validation attributes.
    /// </summary>
    public class ProductOptionGroupViewModel
    {
        public int ProductOptionGroupID { get; set; }
        public int ProductID { get; set; }
        [Required(ErrorMessage = "Option group name is required")]
        [StringLength(100, ErrorMessage = "Option group name cannot exceed 100 characters")]
        public string OptionGroupName { get; set; }
        public bool IsRequired { get; set; } = false;
        public int NumberOfChoice { get; set; } = 1;
        public List<ProductOptionItemViewModel> ProductOptionItems { get; set; } = new List<ProductOptionItemViewModel>();
    }

    /// <summary>
    /// ViewModel for product option items, including validation attributes.
    /// </summary>
    public class ProductOptionItemViewModel
    {
        public int ProductOptionItemsID { get; set; }
        public int ProductOptionGroupID { get; set; }
        [Required(ErrorMessage = "Option name is required")]
        [StringLength(100, ErrorMessage = "Option name cannot exceed 100 characters")]
        public string OptionName { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Additional price cannot be negative")]
        public decimal AdditionalPrice { get; set; } = 0;
    }
}