using System.Collections.Generic;
using System.Linq;
using ASI.Basecode.Data.Models;

namespace ASI.Basecode.WebApp.Areas.Customer.Models
{
    public class HomeViewModel
    {
        public IEnumerable<ProductCategory> Categories { get; set; } = new List<ProductCategory>();
        public IEnumerable<Product> Products { get; set; } = new List<Product>();
    }
}