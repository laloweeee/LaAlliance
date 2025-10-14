using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASI.Basecode.WebApp.Areas.Customer.Models
{
    public class AccountViewModel
    {
        public int UserID { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "email@email.com";
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        public string FirstName { get; set; } = "FirstName";
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
        public string LastName { get; set; } = "LastName";
        [Phone]
        public string ContactNumber { get; set; } = "09123456789";

        // Address List
        public List<UserAddressViewModel> Addresses { get; set; } = new List<UserAddressViewModel>();
    }

    public class UserAddressViewModel
    {
        public int UserAddressID { get; set; }
        public int AddressID { get; set; }
        public bool IsDefault { get; set; } = true;
        [Required]
        [StringLength(50)]
        [Display(Name = "Address Type")]
        public string AddressType { get; set; } = "Home";
        [StringLength(200)]
        [Display(Name = "Address Note")]
        public string AddressNote { get; set; } = "No additional notes";
        [Required]
        [StringLength(200)]
        [Display(Name = "Street")]
        public string Street { get; set; } = "123 Main St";
        [Required]
        [StringLength(100)]
        [Display(Name = "Barangay")]
        public string Barangay { get; set; } = "Barangay 1";
        [Required]
        [StringLength(100)]
        [Display(Name = "City")]
        public string City { get; set; } = "City Name";
        [Required]
        [StringLength(100)]
        [Display(Name = "Province")]
        public string Province { get; set; } = "Cebu";
        [Required]
        [StringLength(20)]
        [Display(Name = "Zip Code")]
        public string ZipCode { get; set; } = "6000";
        public double Longitude { get; set; } = 123.456;
        public double Latitude { get; set; } = 12.345;

        public string FullAddress
        {
            get
            {
                return $"{Street}, {Barangay}, {City}, {Province}, {ZipCode}, Philippines";
            }
        }
    }
}
