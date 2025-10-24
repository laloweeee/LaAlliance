using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static ASI.Basecode.Resources.Constants.Enums;
using System.Linq;

namespace ASI.Basecode.WebApp.Areas.Customer.Models
{
    public class CheckoutViewModel
{
        public Services.ServiceModels.CartViewModel Cart { get; set; }
        public List<UserAddressViewModel> Addresses { get; set; }
        //public List<string> AvailableVouchers { get; set; }
        public int? SelectedAddressId { get; set; }
        public OrderType OrderType { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public decimal SubTotal
        {
            get
            {
                if (Cart?.CartItems == null || !Cart.CartItems.Any())
                    return 0m;

                return Cart.CartItems.Sum(ci =>
                    ci.Quantity * (ci.UnitPrice + (ci.CartItemOptions?.Sum(opt => opt.AdditionalPrice) ?? 0m))
                );
            }
        }
        public decimal DeliveryFee { get; set; } = 50.00m;
        public decimal Total
        {
            get
            {
                return SubTotal + DeliveryFee - DiscountAmount;
            }
        }
        public decimal DiscountAmount { get; set; } = 0.00m;
        public string DeliveryNotes { get; set; }
        public string VoucherCode { get; set; }
    }
}