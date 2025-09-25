using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    public class Address
    {
        public int AddressId { get; set; }
        public int UserId { get; set; }
        public string streetAddress { get; set; }
        public string barangay { get; set; }
        public string city { get; set; }
        public string province { get; set; }
        public int zipCode { get; set; }
        public string country { get;  } = "Philippines";
        public string addressNote { get; set; }
        public bool isDefault { get; set; }

    }
}
