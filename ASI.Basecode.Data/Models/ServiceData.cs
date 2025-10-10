namespace ASI.Basecode.Data.Models
{
    public partial class ServiceData
    {
        public int ServiceDataId { get; set; }
        public int RestaurantId { get; set; }
        public int MaxDeliveryDistance { get; set; }
        public decimal BaseDeliveryFee { get; set; }
        public decimal PerKmFee { get; set; }
        public decimal MinimumOrderAmount { get; set; }

        // Navigator
        public RestaurantProfile RestaurantProfile { get; set; }
    }
}