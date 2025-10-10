using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    public partial class ProductOptionItems
    {
        public int ProductOptionItemsID { get; set; }
        public int ProductOptionGroupID { get; set; }
        public string OptionName { get; set; }
        public decimal AdditionalPrice { get; set; }

        // Navigator
        public ProductOptionGroup ProductOptionGroup { get; set; }
        public CartItemOption CartItemOption { get; set; }
        public ICollection<OrderItemOption> OrderItemOption { get; set; } = [];
    }
}
