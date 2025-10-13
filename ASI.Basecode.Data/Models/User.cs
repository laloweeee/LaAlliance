using System;
using System.Collections.Generic;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Data.Models
{
    public partial class User
    {
        public int UserID { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public UserType UserType { get; set; }
        public bool IsEmailVerified { get; set; } = false;
        public string EmailHashToken { get; set; }
        public string ResetPasswordHashToken { get; set; }
        public AccountStatus AccountStatus { get; set; } = AccountStatus.Active;

        // Navigator
        public UserProfile UserProfile { get; set; } // One user has one profile
        public ICollection<UserAddress> UserAddress { get; set; } = new List<UserAddress>(); // One user can have multiple addresses
        public RestaurantStaff RestaurantStaff { get; set; } // One user can be one restaurant staff
        public Cart Cart { get; set; } // One user can have one cart
        public ICollection<CustomerProductFavorites> CustomerProductFavorites { get; set; } = new List<CustomerProductFavorites>(); // One user can have multiple customer product favorites
        public ICollection<Order> Order { get; set; } = new List<Order>(); // One user can have multiple orders
    }
}
