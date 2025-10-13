using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ASI.Basecode.Services.ServiceModels
{
    public class CartViewModel
    {
        public int CartID { get; set; }
        public int UserID { get; set; }
        public List<CartItemViewModel> CartItems { get; set; } = new List<CartItemViewModel>();
        public decimal TotalAmount => CartItems?.Sum(item => item.TotalPrice) ?? 0;
        public int TotalItems => CartItems?.Sum(item => item.Quantity) ?? 0;
    }

    public class CartItemViewModel
    {
        public int CartItemID { get; set; }
        public int ProductID { get; set; }
        public string ProductImage { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice => (UnitPrice + Options.Sum(o => o.AdditionalPrice)) * Quantity;
        public List<CartItemOptionViewModel> Options { get; set; } = new List<CartItemOptionViewModel>();
    }

    public class CartItemOptionViewModel
    {
        public int CartItemOptionID { get; set; }
        public int ProductOptionGroupID { get; set; }
        public int ProductOptionItemID { get; set; }
        public string OptionGroupName { get; set; }
        public string OptionName { get; set; }
        public decimal AdditionalPrice { get; set; }
    }

    // For adding items to cart
    public class AddToCartRequest
    {
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public Dictionary<int, List<int>> SelectedOptions { get; set; } = new Dictionary<int, List<int>>();
    }
}