using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class ProductCategory
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public bool IsActive { get; set; }

        // Navigator
        public ICollection<Product> Products { get; set; } = new List<Product>(); // One product category can have multiple products
    }
}
