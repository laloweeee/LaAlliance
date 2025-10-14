using System;
using System.Collections.Generic;

namespace ASI.Basecode.WebApp.Models
{
    public class OrderTrackingViewModel
    {
        public CurrentOrderViewModel CurrentOrder { get; set; }
        public List<OrderHistoryViewModel> OrderHistory { get; set; } = new List<OrderHistoryViewModel>();
    }

    public class CurrentOrderViewModel
    {
        public string OrderNumber { get; set; }
        public string Status { get; set; }
        public int ProgressPercentage { get; set; }
        public string ETA { get; set; }
        public List<OrderStepViewModel> OrderSteps { get; set; } = new List<OrderStepViewModel>();
        
        // Location data for map
        public LocationViewModel DeliveryLocation { get; set; }
        public LocationViewModel RestaurantLocation { get; set; }
        public LocationViewModel DriverLocation { get; set; }
    }

    public class OrderStepViewModel
    {
        public string StepName { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsCurrent { get; set; }
        public string Timestamp { get; set; }
        public string IconClass { get; set; }
    }

    public class OrderHistoryViewModel
    {
        public string OrderNumber { get; set; }
        public string Items { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
    }

    public class LocationViewModel
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Address { get; set; }
        public string Title { get; set; }
    }
}
