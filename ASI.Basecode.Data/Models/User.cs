using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ASI.Basecode.Data.Models
{
    public partial class User
    {
        public int UserID { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public bool IsEmailVerified { get; set; }
        public string EmailHashToken { get; set; }
        public DateTime? EmailTokenExpiry { get; set; }
        public string ResetPasswordHashToken { get; set; }
        public DateTime? ResetTokenExpiry { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime UpdatedTime { get; set; }
    }
}
