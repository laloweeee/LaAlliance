#nullable enable
using System;

namespace ASI.Basecode.Data.Models
{
    public partial class EmailVerificationToken
    {
        public int TokenID { get; set; }
        public int UserID { get; set; }
        public string Token { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; } = false;

        // Navigation Property
        public User User { get; set; } = null!;
    }

    public partial class PasswordResetToken
    {
        public int TokenID { get; set; }
        public int? UserID { get; set; }
        public string Token { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; } = false;

        // Navigation Property
        public User? User { get; set; }
    }
}