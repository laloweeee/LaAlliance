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
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string Street { get; set; }
        public string Barangay { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public int ZipCode { get; set; }
        public string Country { get; } = "Philippines";

        //Navigator
        public UserAddress UserAddress { get; set; }
        public RestaurantAddress RestaurantAddress { get; set; }
        public Order Order { get; set; }
    }
}
