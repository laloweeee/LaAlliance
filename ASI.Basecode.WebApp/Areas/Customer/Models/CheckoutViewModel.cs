using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ASI.Basecode.Services.ServiceModels;

namespace ASI.Basecode.WebApp.Areas.Customer.Models
{
    public class CheckoutViewModel
    {
        public ASI.Basecode.Services.ServiceModels.CartViewModel Cart { get; set; }

        [Required(ErrorMessage = "Street address is required.")]
        public string Street { get; set; }

        [Required(ErrorMessage = "Barangay is required.")]
        public string Barangay { get; set; }

        [Required(ErrorMessage = "City is required.")]
        public string City { get; set; }

        [Required(ErrorMessage = "Province is required.")]
        public string Province { get; set; }

        [Required(ErrorMessage = "Zip code is required.")]
        public string ZipCode { get; set; }

        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        // Computed property for full delivery address
        public string DeliveryAddress => $"{Street}, {Barangay}, {City}, {Province} {ZipCode}";

        [Required(ErrorMessage = "Order type is required.")]
        public string OrderType { get; set; } // "Delivery" or "Pickup"

        [Required(ErrorMessage = "Delivery option is required.")]
        public string DeliveryOption { get; set; }

        public string ScheduledDateTime { get; set; }

        // New properties for user information
        [Required(ErrorMessage = "Full name is required.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Contact number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string ContactNumber { get; set; }

        [Required(ErrorMessage = "Payment method is required.")]
        public string PaymentMethod { get; set; }

        public string VoucherCode { get; set; }

        public List<string> AvailableVouchers { get; set; }

        public List<UserAddressViewModel> Addresses { get; set; }
    }
}