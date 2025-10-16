using System;
using System.Collections.Generic;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Data.Models
{
    /// <summary>
    /// User entity with soft delete support
    /// </summary>
    public partial class User : BaseEntity
    {
        public int UserID { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public UserType UserType { get; set; }
        public bool IsEmailVerified { get; set; } = false;
        public AccountStatus AccountStatus { get; set; } = AccountStatus.Active;

        // Navigator
        public UserProfile UserProfile { get; set; } // One user has one profile
        public ICollection<UserAddress> UserAddress { get; set; } = new List<UserAddress>(); // One user can have multiple addresses
        public RestaurantStaff RestaurantStaff { get; set; } // One user can be one restaurant staff
        public Cart Cart { get; set; } // One user can have one cart
        public ICollection<CustomerProductFavorites> CustomerProductFavorites { get; set; } = new List<CustomerProductFavorites>(); // One user can have multiple customer product favorites
        public ICollection<Order> Order { get; set; } = new List<Order>(); // One user can have multiple orders
        public ICollection<EmailVerificationToken> EmailVerificationTokens { get; set; } = new List<EmailVerificationToken>(); // One user can have multiple email verification tokens
        public ICollection<PasswordResetToken> PasswordResetTokens { get; set; } = new List<PasswordResetToken>(); // One user can have multiple password reset tokens
    }
}
