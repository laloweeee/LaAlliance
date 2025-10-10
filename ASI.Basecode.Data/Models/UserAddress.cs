using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    public partial class UserAddress
    {
        public int UserAddressId { get; set; }
        public int UserId { get; set; }
        public int AddressId { get; set; }
        public bool IsDefault { get; set; }
        public string AddressLabel { get; set; }
        public string AddressNote { get; set; }
        public DateTime CreatedAt { get; set; }
        public User User { get; set; }
        public Address Address { get; set; }
    }
}
