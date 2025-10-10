using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    public partial class CartItem
    {
        public int CartProductID { get; set; }
        public int CartID { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        //Navigator
        public CartItemOption CartItemOption { get; set; }
        public Cart Cart { get; set; }
        public Product Product { get; set; }
    }
}
