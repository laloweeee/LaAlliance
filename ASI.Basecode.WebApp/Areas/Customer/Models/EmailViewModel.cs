using System.ComponentModel.DataAnnotations;

namespace ASI.Basecode.WebApp.Areas.Customer.Models
{
    public class EmailViewModel
    {
        [Required(ErrorMessage = "Current email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [Display(Name = "Current Email")]
        public string CurrentEmail { get; set; }

        [Required(ErrorMessage = "New email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [Display(Name = "New Email")]
        public string NewEmail { get; set; }

        [Required(ErrorMessage = "Please confirm your new email.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [Compare("NewEmail", ErrorMessage = "The new email and confirmation email do not match.")]
        [Display(Name = "Confirm New Email")]
        public string ConfirmEmail { get; set; }
    }
}
