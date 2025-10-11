using System.ComponentModel.DataAnnotations;

namespace ASI.Basecode.WebApp.Areas.Restaurant.Models
{
    public class RestaurantProfileViewModel
    {
        [Required, Display(Name = "Restaurant Name")]
        public string RestaurantName { get; set; } = "";

        [Required]
        public string Address { get; set; } = "";

        [Phone]
        public string Phone { get; set; } = "";

        [EmailAddress]
        public string Email { get; set; } = "";

        [Range(0, 1000), Display(Name = "Delivery Radius (miles)")]
        public decimal? DeliveryRadius { get; set; }

        [Range(0, 9999), Display(Name = "Delivery Fee")]
        public decimal? DeliveryFee { get; set; }

        [Range(0, 9999), Display(Name = "Minimum Order Amount")]
        public decimal? MinimumOrderAmount { get; set; }

        [Display(Name = "Opening Time")]
        public string OpenTime { get; set; }

        [Display(Name = "Closing Time")]
        public string CloseTime { get; set; }

        [Display(Name = "Open 24/7?")]
        public bool OpenAllDay { get; set; }
    }
}
