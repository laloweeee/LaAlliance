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
        public Order Order { get; set; }
        public Product Product { get; set; }
        public ICollection<OrderItemOption> OrderItemOption { get; set; } = [];
    }
}
