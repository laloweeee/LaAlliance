

using System;

namespace ASI.Basecode.Services.ServiceModels
{
    public class RestaurantProfileViewModel
    {
        public int RestaurantID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string LogoUrl { get; set; }
        public string CoverImageUrl { get; set; }
        public TimeOnly OpeningTime { get; set; }
        public TimeOnly ClosingTime { get; set; }
    }
}