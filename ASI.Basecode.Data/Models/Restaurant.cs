using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class Restaurant
    {
        public int RestaurantID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public TimeOnly OpeningTime { get; set; } = TimeOnly.Parse("09:00");
        public TimeOnly ClosingTime { get; set; } = TimeOnly.Parse("21:00");

        // Navigator
        public RestaurantAddress RestaurantAddress { get; set; } // One restaurant has one address
    }
}