using ASI.Basecode.Data.Models;
using System.Collections.Generic;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Data.Interfaces
{
    /// <summary>
    /// Interface for Order Repository
    /// </summary>
    public interface IOrderRepository
    {
        /// <summary>
        /// Add a new order
        /// </summary>
        /// <param name="order"></param>
        void AddOrder(Order order);

        /// <summary>
        /// Update an existing order
        /// </summary>
        /// <param name="order"></param>
        void UpdateOrder(Order order);

        /// <summary>
        /// Delete an order by its ID
        /// </summary>
        /// <param name="orderID"></param>
        void DeleteOrder(int orderID);

        /// <summary>
        /// Get an order by its ID
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        Order GetOrderById(int orderID);

        /// <summary>
        /// Get all orders for a specific user
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        IEnumerable<Order> GetOrdersByUserId(int userID);

        /// <summary>
        /// Get all orders
        /// </summary>
        /// <returns></returns>
        IEnumerable<Order> GetAllOrders();

        /// <summary>
        /// Get orders by status
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        IEnumerable<Order> GetOrdersByStatus(OrderStatus status);

        /// <summary>
        /// Get orders by date range
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        IEnumerable<Order> GetOrdersByDateRange(System.DateTime startDate, System.DateTime endDate);
    }
}
