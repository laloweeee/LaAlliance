using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    /// <summary>
    /// Product entity with soft delete support
    /// </summary>
    public partial class Product : BaseEntity
    {
        public int ProductID { get; set; }
        public int CategoryID { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public string ProductDescription { get; set; }
        public decimal ProductPrice { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigator
        public ProductCategory ProductCategory { get; set; } // One product belongs to one product category
        public ICollection<OrderItems> OrderItems { get; set; } = new List<OrderItems>(); // One product can be in multiple order items
        public ICollection<PromotionProducts> PromotionProducts { get; set; } = new List<PromotionProducts>(); // One product can be in multiple promotional products
        public ICollection<ProductOptionGroup> ProductOptionGroup { get; set; } = new List<ProductOptionGroup>(); // One product can have multiple product option groups
        public ICollection<CustomerProductFavorites> CustomerProductFavorites { get; set; } = new List<CustomerProductFavorites>(); // One product can be favorited by multiple customers
    }
}
