using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    public class UserProfile
    {
        public int UserId { get; set; } // UserID of the user
        public string ContactNumber { get; set; } // Contact number of the user
        public string UserPhoto { get; set; } // URL or path to the user's photo
        public DateTime CreatedTime { get; set; } // Timestamp of creation
        public DateTime UpdatedTime { get; set; } // Timestamp of last update
    }
}
