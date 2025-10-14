using System.Collections.Generic;

namespace ASI.Basecode.Services.ServiceModels
{
    public class UserProfileServiceModel
    {
        public int UserID { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ContactNumber { get; set; }

        // Address List
        public List<UserAddressServiceModel> Addresses { get; set; } = new List<UserAddressServiceModel>();
    }

    public class UserAddressServiceModel
    {
        public int UserAddressID { get; set; }
        public int UserID { get; set; }
        public int AddressID { get; set; }
        public bool IsDefault { get; set; }
        public string AddressType { get; set; }
        public string AddressNote { get; set; }
        // Address Details
        public string Street { get; set; }
        public string Barangay { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string ZipCode { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }
}