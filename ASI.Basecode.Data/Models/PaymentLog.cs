#nullable enable
using static ASI.Basecode.Resources.Constants.Enums;
using System;

namespace ASI.Basecode.Data.Models
{
    public class PaymentLog
    {
        public int PaymentLogID { get; set; }
        public int OrderID { get; set; }
        public int? UserID { get; set; }
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.GCash;
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
        public string TransactionReference { get; set; } = null!;
        public decimal PaymentAmount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Remarks { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        // Navigators
        public Order Order { get; set; } = null!; // One payment log is associated with one order
        public User? User { get; set; } // One payment log is associated with one user
    }
}