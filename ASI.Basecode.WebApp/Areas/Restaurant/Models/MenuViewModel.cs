using ASI.Basecode.Data.Models;
using System.Collections.Generic;

namespace ASI.Basecode.WebApp.Areas.Restaurant.Models
{
    public class MenuViewModel
    {
        public IEnumerable<ProductCategory> Categories { get; set; } = new List<ProductCategory>();
        public IEnumerable<Product> Products { get; set; } = new List<Product>();
    }
}