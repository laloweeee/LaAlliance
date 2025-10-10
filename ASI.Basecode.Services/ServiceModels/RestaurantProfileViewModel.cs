

using System;
using System.ComponentModel.DataAnnotations;

namespace ASI.Basecode.Services.ServiceModels
{
    public class RestaurantProfileViewModel
    {
        public int RestaurantID { get; set; }
        [Required, Display(Name = "Restaurant Name")]
        public string Name { get; set; } = "";

        [Required, MaxLength(500)]
        public string Description { get; set; } = "";

        [Phone]
        public string PhoneNumber { get; set; } = "";

        [EmailAddress]
        public string Email { get; set; } = "";

        [Display(Name = "Opening Time")]
        public TimeOnly OpeningTime { get; set; }

        [Display(Name = "Closing Time")]
        public TimeOnly ClosingTime { get; set; }

        public AddressViewModel Address { get; set; }
    }
}