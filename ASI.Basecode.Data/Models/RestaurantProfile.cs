using System;

namespace ASI.Basecode.Data.Models
{
    public partial class RestaurantProfile
    {
        public int RestaurantID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public int AddressId { get; set; }
        public TimeOnly OpeningTime { get; set; }
        public TimeOnly ClosingTime { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime UpdatedTime { get; set; }
        public int UpdatedBy { get; set; }

        // Navigator
        public User UpdatedByUser { get; set; }
        public Address Address { get; set; }
    }
}