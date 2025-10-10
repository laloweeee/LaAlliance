using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    public partial class ProductOptionGroup
    {
        public int ProductOptionGroupID { get; set; }
        public int ProductID { get; set; }
        public string OptionGroupName { get; set; }
        public bool IsRequired { get; set; }
        public int NumberOfChoice { get; set; }

        // Navigator
        public Product Product { get; set; }
        public CartItemOption CartItemOption { get; set; }
        public ICollection<ProductOptionItems> ProductOptionItems { get; set; } = [];
        public ICollection<OrderItemOption> OrderItemOption { get; set; } = [];
    }
}
