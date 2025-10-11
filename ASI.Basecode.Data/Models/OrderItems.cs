using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    public partial class OrderItems
    {
        public int OderItemID { get; set; }
        public int OderID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        //Navigator
        public Order Order { get; set; } // One order item belongs to one order
        public Product Product { get; set; } // One order item is for one product
        public ICollection<OrderItemOption> OrderItemOption { get; set; } = new List<OrderItemOption>(); // One order item can have multiple order item options
    }
}
