using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    public partial class UserProfile
    {
        public int UserProfileId { get; set; }
        public int UserId { get; set; }
        public string ContactNumber { get; set; }
        public string UserPhoto { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime UpdatedTime { get; set; }

        // Navigator
        public User User { get; set; }
    }
}
