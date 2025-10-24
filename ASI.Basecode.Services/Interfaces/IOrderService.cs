using ASI.Basecode.Services.ServiceModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderViewModel> PlaceOrder(PlaceOrderRequest request);
        OrderViewModel GetOrderById(int orderID);
        IEnumerable<OrderViewModel> GetOrdersByUserId(int userID);
        List<OrderViewModel> GetAllOrders();
        void CancelOrder(int orderID, int userID);
        Task UpdateOrderStatus(int orderID, int userID, OrderStatus newStatus);
        
        // Methods for dashboard
        Task<DashboardStatsViewModel> GetDashboardStatsAsync();
        Task<OrderSummaryViewModel> GetOrderSummaryAsync();
        Task<decimal> GetTotalSalesAsync();
        Task<decimal> GetAverageSalePerDayAsync();

        Task<List<StaffActivityViewModel>> GetRecentStaffActivitiesAsync(int count = 10);
        Task<string> GetAverageProcessingTimeAsync();
    }
}