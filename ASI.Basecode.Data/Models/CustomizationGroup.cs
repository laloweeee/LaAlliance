using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    public class CustomizationGroup
    {
        public int CustomizationGroupId { get; set; }
        public int ProductId { get; set; }
        public string CustomizationName { get; set; }
        public bool IsRequired { get; set; }
        public bool IsSingleChoice { get; set; }
        public int CreatedBy { get; set; } // UserID of the creator
        public DateTime CreatedTime { get; set; } // Timestamp of creation
        public int UpdatedBy { get; set; } // UserID of the last updater
        public DateTime UpdatedTime { get; set; } // Timestamp of last update
    }
}
