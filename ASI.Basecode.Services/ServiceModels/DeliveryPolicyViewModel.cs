namespace ASI.Basecode.Services.ServiceModels
{
    public class ServiceDeliveryViewModel
    {
        public int MaxDeliveryDistance { get; set; }
        public decimal BaseDeliveryFee { get; set; }
        public decimal PerKmFee { get; set; }
        public decimal MinimumOrderAmount { get; set; }
    }
}