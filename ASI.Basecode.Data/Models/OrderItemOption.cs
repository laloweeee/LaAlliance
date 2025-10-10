using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    public partial class OrderItemOption
    {
        public int OrderItemOptionID { get; set; }
        public int OrderItemID{ get; set; }
        public int ProductOptionGroupID { get; set; }
        public int ProductOptionItemID { get; set; }

        //Navigator
        public OrderItems OrderItems { get; set; }
        public ProductOptionGroup ProductOptionGroup { get; set; }
        public ProductOptionItems ProductOptionItems { get; set; }
    }
}
