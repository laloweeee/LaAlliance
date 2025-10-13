using System;
using System.ComponentModel.DataAnnotations;

namespace ASI.Basecode.Services.ServiceModels
{
    public class RestaurantViewModel
    {
        public int RestaurantID { get; set; }
        [Required, Display(Name = "Restaurant Name")]
        public string Name { get; set; }

        [Required, MaxLength(500)]
        public string Description { get; set; }

        [Phone]
        public string ContactNumber { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Opening Time")]
        public TimeOnly OpeningTime { get; set; }

        [Display(Name = "Closing Time")]
        public TimeOnly ClosingTime { get; set; }
        public AddressViewModel Address { get; set; } = new AddressViewModel();
    }

    public class AddressViewModel
    {
        public int AddressID { get; set; }
        public string Street { get; set; }
        public string Barangay { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}