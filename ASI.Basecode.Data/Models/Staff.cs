using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    public partial class Staff
    {
        public int StaffId { get; set; }
        public int UserId { get; set; }
        public int RestaurantId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string RestaurantPermission { get; set; }
        public string Status { get; set; }

        // Refers to the restaurant admin who created this staff record.
        public int CreatedBy { get; set; } 
        public DateTime CreatedTime { get; set; }

        // Refers to the user who last updated this staff record.
        public int UpdatedBy { get; set; } 
        public DateTime UpdatedTime { get; set; }
    }
}
