#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    public partial class ProductOptionGroup
    {
        public int ProductOptionGroupID { get; set; }
        public int? ProductID { get; set; }
        public string OptionGroupName { get; set; } = null!;
        public bool IsRequired { get; set; }
        public int NumberOfChoice { get; set; }

        // Navigator
        public Product? Product { get; set; } // One product option group belongs to one product
        public ICollection<CartItemOption> CartItemOptions { get; set; } = new List<CartItemOption>(); // One product option group can be associated with multiple cart item options
        public ICollection<ProductOptionItems> ProductOptionItems { get; set; } = new List<ProductOptionItems>(); // One product option group can have multiple product option items
        public ICollection<OrderItemOption> OrderItemOption { get; set; } = new List<OrderItemOption>(); // One product option group can have multiple order item options
    }
}
