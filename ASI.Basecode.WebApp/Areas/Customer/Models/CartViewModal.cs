using System;
using System.Collections.Generic;
using System.Linq;

namespace ASI.Basecode.WebApp.Areas.Customer.Models
{
    public class CartViewModel
    {
        public int CartID { get; set; }
        public int UserID { get; set; }
        public int TotalItems => CartItems.Sum(ci => ci.Quantity);
        public decimal SubTotal => CartItems.Sum(ci => ci.Quantity * (ci.UnitPrice + ci.CartItemOptions.Sum(opt => opt.AdditionalPrice)));
        public decimal DeliveryFee { get; set; } = 50.00m;
        public decimal Total => SubTotal + DeliveryFee;
        public List<CartItemViewModel> CartItems { get; set; } = new List<CartItemViewModel>();

        public static implicit operator CartViewModel(Services.ServiceModels.CartViewModel v)
        {
            throw new NotImplementedException();
        }
    }

    public class CartItemViewModel
    {
        public int CartItemID { get; set; }
        public int ProductID { get; set; }
        public string ProductImage { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; } 
        public int Quantity { get; set; }
        public decimal ItemAddOnsTotal => CartItemOptions.Sum(opt => opt.AdditionalPrice);
        public decimal TotalPrice => Quantity * (UnitPrice + ItemAddOnsTotal);
        public List<CartItemOptionViewModel> CartItemOptions { get; set; } = new List<CartItemOptionViewModel>();
        public List<CartItemOptionViewModel> SelectedOptions { get; set; }
    }

    public class CartItemOptionViewModel
    {
        public int CartItemOptionID { get; set; }
        public int CartItemID { get; set; }
        public int ProductOptionGroupID { get; set; }
        public string OptionGroupName { get; set; }
        public int ProductOptionItemID { get; set; }
        public string OptionName { get; set; }
        public decimal AdditionalPrice { get; set; }
        
    }
}