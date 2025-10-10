using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    public partial class CustomizationGroup
    {
        public int CustomizationGroupID { get; set; }
        public int ProductID { get; set; }
        public string CustomizationName { get; set; }
        public bool IsRequired { get; set; }
        public int NumberOfChoice { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime UpdatedTime { get; set; }

        // Navigator
        public Product Product { get; set; }

        // Collection
        public ICollection<CustomizationOption> CustomizationOptions { get; set; } = [];
    }
}
