using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    public partial class Product
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int CategoryID { get; set; }
        public string ProductImage { get; set; }
        public string ProductDescription { get; set; }
        public decimal ProductPrice { get; set; }
        public bool IsAvailable { get; set; }

        // Navigator
        public CustomerProductFavorites CustomerProductFavorites { get; set; }
        public ProductCategory ProductCategory { get; set; }
        public ICollection<OrderItems> OrderItems { get; set; } = [];
        public ICollection<PromotionProducts> PromotionProducts { get; set; } = [];
        public ICollection<ProductOptionGroup> ProductOptionGroup { get; set; } = [];
    }
}
