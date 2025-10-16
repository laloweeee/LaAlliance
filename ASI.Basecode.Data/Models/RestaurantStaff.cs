using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Data.Models
{
    /// <summary>
    /// Restaurant Staff entity with soft delete support
    /// </summary>
    public partial class RestaurantStaff : BaseEntity
    {
        public int StaffID { get; set; }
        public int UserID  { get; set; }
        public StaffRole? Role { get; set; }
        public AccountStatus Status { get; set; }

        //Navigator
        public User User { get; set; } // One restaurant staff is one user
        public ICollection<OrderProcessed> OrderProcessed { get; set; } = new List<OrderProcessed>(); // One restaurant staff can have multiple order processed
    }
}
