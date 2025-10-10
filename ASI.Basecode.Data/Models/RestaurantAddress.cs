using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    public partial class RestaurantAddress
    {
        public int RestaurantAddressID { get; set; }
        public int RestaurantID { get; set; }
        public int AddressID { get; set; }

        //Navigator
        public Restaurant Restaurant { get; set; }
        public Address Address { get; set; }
    }
}
