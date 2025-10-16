using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ASI.Basecode.Services.ServiceModels;

namespace ASI.Basecode.WebApp.Areas.Customer.Models
{
    public class CheckoutViewModel
    {
        public ASI.Basecode.Services.ServiceModels.CartViewModel Cart { get; set; }

        [Required(ErrorMessage = "Delivery address is required.")]
        public string DeliveryAddress { get; set; }

        [Required(ErrorMessage = "Delivery option is required.")]
        public string DeliveryOption { get; set; }

        public string ScheduledDateTime { get; set; }

        // New properties for user information
        [Required(ErrorMessage = "Full name is required.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }  // User's email from profile

        [Required(ErrorMessage = "Contact number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string ContactNumber { get; set; }  // User's contact number from profile

        [Required(ErrorMessage = "Payment method is required.")]
        public string PaymentMethod { get; set; }

        public string VoucherCode { get; set; }

        public List<string> AvailableVouchers { get; set; }

        public List<UserAddressViewModel> Addresses { get; set; }
    }
}
