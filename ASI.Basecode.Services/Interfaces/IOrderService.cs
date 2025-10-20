using ASI.Basecode.Services.ServiceModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Services.Interfaces
{
    /// <summary>
    /// Interface for Order Service operations
    /// </summary>
    public interface IOrderService
    {
        Task<OrderViewModel> PlaceOrder(PlaceOrderRequest request);
        OrderViewModel GetOrderById(int orderID);
        IEnumerable<OrderViewModel> GetOrdersByUserId(int userID);
        List<OrderViewModel> GetAllOrders();
        void CancelOrder(int orderID, int userID);
        Task UpdateOrderStatus(int orderID, int userID, OrderStatus newStatus);
    }
}
