using System.ComponentModel.DataAnnotations;

namespace ASI.Basecode.WebApp.Areas.Customer.Models
{
    public class PasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }
        [Required]
        [MinLength(8, ErrorMessage = "The new password must be at least 8 characters long.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [Required]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}