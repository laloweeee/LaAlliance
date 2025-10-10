using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    public partial class UserAddress
    {
        public int UserAddressID { get; set; }
        public int UserID { get; set; }
        public int AddressID { get; set; }
        public bool IsDefault { get; set; }
        public string AddressType { get; set; }
        public string AddressNote { get; set; }

        //Navigator
        public Address Address { get; set; }
        public User User { get; set; }
    }
}
