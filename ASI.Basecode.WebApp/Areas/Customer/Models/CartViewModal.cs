using System.Collections.Generic;
using System.Linq;

namespace ASI.Basecode.WebApp.Areas.Customer.Models
{
    public class CartViewModel
    {
        public int CartID { get; set; }
        public int UserID { get; set; }
        public int TotalItems => CartItems.Sum(ci => ci.Quantity);
        public decimal TotalPrice => CartItems.Sum(ci => ci.Quantity * (ci.ProductPrice + ci.CartItemOptions.Sum(opt => opt.AdditionalPrice)));
        public List<CartItemViewModel> CartItems { get; set; } = new List<CartItemViewModel>();
    }

    public class CartItemViewModel
    {
        public int CartItemID { get; set; }
        public int CartID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public int Quantity { get; set; }
        public List<CartItemOptionViewModel> CartItemOptions { get; set; } = new List<CartItemOptionViewModel>();
    }

    public class CartItemOptionViewModel
    {
        public int CartItemOptionID { get; set; }
        public int CartItemID { get; set; }
        public int ProductOptionGroupID { get; set; }
        public string OptionGroupName { get; set; }
        public int ProductOptionItemsID { get; set; }
        public string OptionName { get; set; }
        public decimal AdditionalPrice { get; set; } = 0;
    }
}