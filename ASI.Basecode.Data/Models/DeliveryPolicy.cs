using System;

namespace ASI.Basecode.Data.Models
{
    /// <summary>
    /// Delivery Policy entity with soft delete support
    /// </summary>
    public partial class DeliveryPolicy : BaseEntity
    {
        public int PolicyID { get; set; }
        public int MaxDeliveryDistance { get; set; }
        public decimal BaseDeliveryFee { get; set; }
        public decimal PerKmFee { get; set; }
        public decimal MinimumOrderAmount { get; set; }
        public DateTime EffectiveDate { get; set; }
        public bool IsActive { get; set; }
    }
}