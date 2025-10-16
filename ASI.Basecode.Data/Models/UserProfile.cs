using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    /// <summary>
    /// User Profile entity with soft delete support
    /// </summary>
    public partial class UserProfile : BaseEntity
    {
        public int ProfileID { get; set; }
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ContactNumber { get; set; }

        // Navigator
        public User User { get; set; } // One user has one profile
    }
}
