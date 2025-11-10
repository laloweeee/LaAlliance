using System;

namespace ASI.Basecode.Services.ServiceModels
{
    /// <summary>
    /// Result returned when validating a promotion code against a cart.
    /// </summary>
    public class PromotionValidationResult
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }
        public decimal DiscountAmount { get; set; }
        public int? PromotionID { get; set; }
        public int? PromotionCodeID { get; set; }
    }
}
