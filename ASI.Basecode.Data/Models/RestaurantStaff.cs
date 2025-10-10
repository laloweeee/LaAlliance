using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    public partial class RestaurantStaff
    {
        public int StaffID { get; set; }
        public int UserID  { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }

        //Navigator
        public User User { get; set; }
        public ICollection<OrderProcessed> OrderProcessed { get; set; } = [];
    }
}
