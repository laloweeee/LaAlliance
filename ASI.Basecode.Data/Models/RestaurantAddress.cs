using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    /// <summary>
    /// Restaurant Address entity with soft delete support
    /// </summary>
    public partial class RestaurantAddress : BaseEntity
    {
        public int RestaurantAddressID { get; set; }
        public int RestaurantID { get; set; }
        public int AddressID { get; set; }

        //Navigator
        public Restaurant Restaurant { get; set; } // One restaurant address belongs to one restaurant
        public Address Address { get; set; } // One restaurant address belongs to one address
    }
}
