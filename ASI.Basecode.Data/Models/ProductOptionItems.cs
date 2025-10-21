using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    public partial class ProductOptionItems : BaseEntity
    {
        public int ProductOptionItemsID { get; set; }
        public int ProductOptionGroupID { get; set; }
        public string OptionName { get; set; }
        public decimal AdditionalPrice { get; set; }

        // Navigator
        public ProductOptionGroup ProductOptionGroup { get; set; } // One product option item belongs to one product option group
        public ICollection<CartItemOption> CartItemOptions { get; set; } = new List<CartItemOption>(); // One product option item can be associated with multiple cart item options
        public ICollection<OrderItemOption> OrderItemOption { get; set; } = new List<OrderItemOption>(); // One product option item can be associated with multiple order item options
    }
}
