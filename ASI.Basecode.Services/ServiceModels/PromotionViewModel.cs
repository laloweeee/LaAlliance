using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Services.ServiceModels
{
    /// <summary>
    /// ViewModel for promotions, including validation attributes and nested promotion codes and products.
    /// </summary>
    public class PromotionViewModel
    {
        public int PromotionID { get; set; }

        [Required(ErrorMessage = "Promotion name is required")]
        [StringLength(200, ErrorMessage = "Promotion name cannot exceed 200 characters")]
        public string PromotionName { get; set; }

        [Display(Name = "Promotion Banner")]
        public IFormFile PromotionalBannerFile { get; set; }
        public string PromotionBanner { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string PromotionDescription { get; set; }

        [Required(ErrorMessage = "Discount type is required")]
        public DiscountType DiscountType { get; set; } = DiscountType.Percentage;

        [Required(ErrorMessage = "Discount value is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Discount value must be greater than 0")]
        public decimal DiscountValue { get; set; }

        [Required(ErrorMessage = "Minimum order amount is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Minimum order amount cannot be negative")]
        public decimal MinimumOrderAmount { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        // Promotion Code properties
        [StringLength(50, ErrorMessage = "Promotion code cannot exceed 50 characters")]
        public string Code { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Usage limit must be at least 1")]
        public int UsageLimit { get; set; } = 1;

        public int UsedCount { get; set; } = 0;

        public DateTime ExpirationDate { get; set; }

        // Product selection
        public List<int> ProductIDs { get; set; } = new List<int>();
    }
}
