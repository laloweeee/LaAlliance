using System.ComponentModel.DataAnnotations;

namespace ASI.Basecode.Services.ServiceModels
{
    public class CategoryViewModel
    {
        public int CategoryID { get; set; }
        [Required(ErrorMessage = "Category name is required")]

        [StringLength(100, ErrorMessage = "Category name cannot exceed 100 characters")]
        public string CategoryName { get; set; }
        public bool IsActive { get; set; }
    }
}