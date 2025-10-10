using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    public partial class CartItemOption
    {
        public int CartItemOptionID { get; set; }
        public int ProductOptionGroupID { get; set; }
        public int ProductOptionItemID { get; set; }

        //Navigator
        public CartItem CartItem { get; set; }
        public ProductOptionGroup ProductOptionGroup { get; set; }
        public ProductOptionItems ProductOptionItems { get; set; }
    }
}
