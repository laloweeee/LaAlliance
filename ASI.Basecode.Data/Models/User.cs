using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Models
{
    public partial class User
    {
        public int UserID { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string UserType { get; set; }
        public bool IsEmailVerified { get; set; }
        public string EmailHashToken { get; set; }
        public DateTime EmailTokenExpiry { get; set; }
        public string ResetPasswordHashToken { get; set; }
        public string ResetPasswordHashTokenExpiry { get; set; }

        // Navigator
        public UserProfile UserProfile { get; set; }
        public UserAddress UserAddress { get; set; }
        public RestaurantStaff RestaurantStaff { get; set; }
        public ICollection<CustomerProductFavorites> CustomerProductFavorites { get; set; } = [];
        public ICollection<Cart> Cart { get; set; } = [];
        public ICollection<Order> Order { get; set; } = [];
    }
}
