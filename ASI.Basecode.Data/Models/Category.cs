using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public int RestaurantId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CreatedBy { get; set; } // UserID of the creator
        public DateTime CreatedTime { get; set; } // Timestamp of creation
        public int UpdatedBy { get; set; } // UserID of the last updater
        public DateTime UpdatedTime { get; set; } // Timestamp of last update
    }
}
