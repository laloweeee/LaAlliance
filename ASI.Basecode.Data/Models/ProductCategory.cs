using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    /// <summary>
    /// Product Category entity with soft delete support
    /// </summary>
    public partial class ProductCategory : BaseEntity
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public bool IsActive { get; set; }

        // Navigator
        public ICollection<Product> Products { get; set; } = new List<Product>(); // One product category can have multiple products
    }
}
