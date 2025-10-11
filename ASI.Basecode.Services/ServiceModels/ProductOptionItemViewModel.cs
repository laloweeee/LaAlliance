using System.ComponentModel.DataAnnotations;

namespace ASI.Basecode.Services.ServiceModels
{
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