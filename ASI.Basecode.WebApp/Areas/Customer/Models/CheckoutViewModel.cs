using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ASI.Basecode.Services.ServiceModels;

namespace ASI.Basecode.WebApp.Areas.Customer.Models
{
    public class CheckoutViewModel
    {
        // Use the service model directly
        public ASI.Basecode.Services.ServiceModels.CartViewModel Cart { get; set; }

        [Required]
        public string DeliveryAddress { get; set; }
        [Required]
        public string DeliveryOption { get; set; }
        public string ScheduledDateTime { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required]
        public string ContactNumber { get; set; }
        [Required]
        public string PaymentMethod { get; set; }
        public string VoucherCode { get; set; }
        public List<string> AvailableVouchers { get; set; }
    }
}