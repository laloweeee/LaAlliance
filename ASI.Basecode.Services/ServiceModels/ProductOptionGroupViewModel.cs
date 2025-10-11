using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASI.Basecode.Services.ServiceModels
{
    public class ProductOptionGroupViewModel
    {
        public int ProductOptionGroupID { get; set; }
        public int ProductID { get; set; }
        [Required(ErrorMessage = "Option group name is required")]
        [StringLength(100, ErrorMessage = "Option group name cannot exceed 100 characters")]
        public string OptionGroupName { get; set; }

        public bool IsRequired { get; set; }

        public int NumberOfChoice { get; set; } = 1;

        public List<ProductOptionItemViewModel> ProductOptionItems { get; set; } = new List<ProductOptionItemViewModel>();
    }
}