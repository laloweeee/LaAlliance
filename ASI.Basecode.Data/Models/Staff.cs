using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Models
{
    public class Staff
    {
        public int Id { get; set; }

        // FK dapat same sa User table
        public int UserID { get; set; }
        public User User { get; set; } = null!;

        // Profile
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string? Phone { get; set; }
        public string? Address { get; set; }

        // Display/HR fields
        public string Role { get; set; } = "Cashier"; // Manager/Chef/Cashier/etc.
        public string Status { get; set; } = "Active"; // Active/Inactive/On Leave
        public DateTime? HireDate { get; set; }
        public decimal? AnnualSalary { get; set; }

        // mo update og mo log-in si user
        public DateTime? LastLoginUtc { get; set; }
    }
}