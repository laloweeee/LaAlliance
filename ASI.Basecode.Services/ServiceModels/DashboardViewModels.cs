using System;
using System.Collections.Generic;

namespace ASI.Basecode.Services.ServiceModels
{
    public class DashboardStatsViewModel
    {
        public int TodaysOrders { get; set; }
        public int TotalOrders { get; set; }
        public int TodaysCustomers { get; set; }
        public int TotalCustomers { get; set; }
        public decimal TotalSales { get; set; }
        public decimal AverageSalePerDay { get; set; }
        public string AverageProcessingTime { get; set; }
    }

    public class OrderSummaryViewModel
    {
        public int Pending { get; set; }
        public int Processing { get; set; }
        public int ReadyForDeliveryPickup { get; set; }
        public int Completed { get; set; }
        public int Cancelled { get; set; }
    }

    public class MostSellingItemViewModel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public int TotalQuantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class MostFavoriteItemViewModel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public int FavoriteCount { get; set; }
        public decimal Price { get; set; }
    }

    public class StaffActivityViewModel
    {
        public int OrderID { get; set; }
        public string StaffName { get; set; }
        public string ElapsedTime { get; set; }
        public DateTime ProcessedAt { get; set; }
        public string CustomerName { get; set; }
        public string OrderType { get; set; }
    }

    public class DashboardDataViewModel
    {
        public DashboardStatsViewModel Stats { get; set; }
        public OrderSummaryViewModel OrderSummary { get; set; }
        public List<MostSellingItemViewModel> MostSellingItems { get; set; }
        public List<MostFavoriteItemViewModel> MostFavoriteItems { get; set; }
        public List<StaffActivityViewModel> StaffActivities { get; set; }
    }
}