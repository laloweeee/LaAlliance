using System.Collections.Generic;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.ServiceModels;
using static ASI.Basecode.Resources.Constants.Enums;
using ASI.Basecode.WebApp.Areas.Customer.Models;

namespace ASI.Basecode.WebApp.Areas.Customer.Models
{
    public class HomeViewModel
    {
        public IEnumerable<ProductCategory> Categories { get; set; } = new List<ProductCategory>();
        public IEnumerable<ProductViewModel> Products { get; set; } = new List<ProductViewModel>();
        public CartViewModel Carts { get; set; } = new CartViewModel();

        // Add these properties for favorites
        public FavoriteServiceModel Favorites { get; set; }
        public int FavoriteCount => Favorites?.TotalItems ?? 0;

        // Add promotions for the banner
        public IEnumerable<RestaurantPromotions> Promotions { get; set; } = new List<RestaurantPromotions>();

        // Map of ProductID -> discounted price (effective price after promotion). If not present, no discount.
        public System.Collections.Generic.IDictionary<int, decimal> ProductDiscounts { get; set; } = new System.Collections.Generic.Dictionary<int, decimal>();
    }
}