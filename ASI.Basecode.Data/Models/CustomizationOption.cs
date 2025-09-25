using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    public class CustomizationOption
    {
        public int CustomizationOptionId { get; set; }
        public int CustomizationGroupId { get; set; }
        public string OptionName { get; set; }
        public decimal AdditionalPrice { get; set; }
        public int CreatedBy { get; set; } // UserID of the creator
        public DateTime CreatedTime { get; set; } // Timestamp of creation
        public int UpdatedBy { get; set; } // UserID of the last updater
        public DateTime UpdatedTime { get; set; } // Timestamp of last update
    }
}
