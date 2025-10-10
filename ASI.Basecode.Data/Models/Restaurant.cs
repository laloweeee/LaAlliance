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
        public TimeOnly OpeningTime { get; set; }
        public TimeOnly ClosingTime { get; set; }

        // Navigator
        public RestaurantAddress RestaurantAddress { get; set; }
        public ICollection<RestaurantPromotions> RestaurantPromotions { get; set; } = [];
    }
}