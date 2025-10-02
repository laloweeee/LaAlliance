using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace ASI.Basecode.Services.ServiceModels
{
    public class CustomizationGroupViewModel
    {
        public int CustomizationGroupID { get; set; }

        [Required(ErrorMessage = "Customization name is required")]
        [StringLength(100, ErrorMessage = "Customization name cannot exceed 100 characters")]
        public string CustomizationName { get; set; }

        public bool IsRequired { get; set; }

        public int NumberOfChoice { get; set; } = 1;

        public int ProductID { get; set; }

        public List<CustomizationOptionViewModel> CustomizationOptions { get; set; } = new List<CustomizationOptionViewModel>();
    }
}