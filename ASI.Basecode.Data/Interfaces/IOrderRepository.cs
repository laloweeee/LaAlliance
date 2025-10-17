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
        /// Get all orders
        /// </summary>
        /// <returns></returns>
        IEnumerable<Order> GetAllOrders();

        Order GetOrderById(int orderID);
    }
}
