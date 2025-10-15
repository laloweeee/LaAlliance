using System.ComponentModel.DataAnnotations;

namespace ASI.Basecode.WebApp.Areas.Customer.Models
{
    public class OTPVerificationViewModel
    {
        [Required(ErrorMessage = "OTP is required.")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "OTP must be 6 digits.")]
        public string OTPCode { get; set; }

        [Required]
        public string VerificationType { get; set; } // "password" or "email" or "new-email"

        // For email change flow
        public string NewEmail { get; set; }
    }
}
