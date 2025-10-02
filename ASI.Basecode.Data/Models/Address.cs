using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    public partial class Address
    {
        public int AddressId { get; set; }
        public string StreetAddress { get; set; }
        public string Barangay { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public int ZipCode { get; set; }
        public string Country { get; } = "Philippines";

        // Navigator
        public ICollection<UserAddress> UserAddresses { get; set; }
        public RestaurantProfile RestaurantProfile { get; set; }
    }
}
