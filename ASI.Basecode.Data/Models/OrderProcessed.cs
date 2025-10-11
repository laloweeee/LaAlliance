using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    public partial class OrderProcessed
    {
        public int OrderProcessID { get; set; }
        public int OrderID { get; set; }
        public int UserID { get; set; }
        public TimeOnly ElapsedTime { get; set; }
        public DateTime ProcessedAt { get; set; }

        //Navigator
        public Order Order { get; set; } // One order process belongs to one order
        public RestaurantStaff HandledBy { get; set; } // One order process is handled by one restaurant staff
    }
}
