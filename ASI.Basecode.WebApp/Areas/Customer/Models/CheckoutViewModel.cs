using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ASI.Basecode.Services.ServiceModels;

namespace ASI.Basecode.WebApp.Areas.Customer.Models
{
    public class CheckoutViewModel
{
        public Services.ServiceModels.CartViewModel Cart { get; set; }
        public List<UserAddressViewModel> Addresses { get; set; }
        public int? SelectedAddressId { get; set; }
        
        [Required(ErrorMessage = "Order type is required.")]
        public string OrderType { get; set; }
        
        [Required(ErrorMessage = "Payment method is required.")]
        public string PaymentMethod { get; set; }
        
        public string DeliveryNotes { get; set; }
        public string VoucherCode { get; set; }
        public List<string> AvailableVouchers { get; set; }

        // ADD THESE PROPERTIES FOR DISPLAY
        public string FullName { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
    }
}