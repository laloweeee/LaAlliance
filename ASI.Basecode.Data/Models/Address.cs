using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Data.Models
{
    /// <summary>
    /// Address entity with soft delete support
    /// </summary>
    public partial class Address : BaseEntity
    {
        public int AddressID { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string Street { get; set; }
        public string Barangay { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public int ZipCode { get; set; }
        public string Country { get; } = "Philippines";

        //Navigator
        public ICollection<UserAddress> UserAddresses { get; set; } // One address can be used by multiple users
        public RestaurantAddress RestaurantAddress { get; set; } // One address can belong to one restaurant
        public Order Order { get; set; } // One address can belong to one order
    }
}
