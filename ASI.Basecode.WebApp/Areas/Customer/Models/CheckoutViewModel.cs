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
        [Required(ErrorMessage = "Delivery notes are required.")]
        public string DeliveryNotes { get; set; }
        public string VoucherCode { get; set; }
        public List<string> AvailableVouchers { get; set; }
    }
}