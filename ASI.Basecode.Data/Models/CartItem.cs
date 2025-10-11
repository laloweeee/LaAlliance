using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    public partial class CartItem
    {
        public int CartItemID { get; set; }
        public int CartID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        //Navigator
        public Cart Cart { get; set; } // One cart item belongs to one cart
        public Product Product { get; set; } // One cart item is for one product
        public ICollection<CartItemOption> CartItemOption { get; set; } = new List<CartItemOption>(); // One cart item can have multiple cart item options
    }
}
