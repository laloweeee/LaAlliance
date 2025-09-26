using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    public class CustomizationOption
    {
        public int CustomizationOptionID { get; set; }
        public int CustomizationGroupID { get; set; }
        public string OptionName { get; set; }
        public decimal AdditionalPrice { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime UpdatedTime { get; set; }

        // Navigator
        public CustomizationGroup CustomizationGroup { get; set; }
    }
}
