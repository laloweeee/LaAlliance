using System.Collections.Generic;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.ServiceModels;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.WebApp.Areas.Customer.Models
{
    public class HomeViewModel
    {
        public IEnumerable<ProductCategory> Categories { get; set; } = new List<ProductCategory>();
        public IEnumerable<ProductViewModel> Products { get; set; } = new List<ProductViewModel>();
        public CartViewModel Carts { get; set; } = new CartViewModel();
    }
}