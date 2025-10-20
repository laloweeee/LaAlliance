using ASI.Basecode.Data.Models;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IOrderProcessedRepository
    {
        /// <summary>
        /// Get all OrderProcesseds
        /// </summary>
        /// <returns></returns>
        IEnumerable<OrderProcessed> GetAllOrderProcesseds();
        /// <summary>
        /// Get OrderProcessed by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        OrderProcessed GetOrderProcessedById(int id);
        /// <summary>
        /// Add a new OrderProcessed
        /// </summary>
        /// <param name="orderProcessed"></param>
        void AddOrderProcessed(OrderProcessed orderProcessed);
        /// <summary>
        /// Update an existing OrderProcessed
        /// </summary>
        /// <param name="orderProcessed"></param>
        void UpdateOrderProcessed(OrderProcessed orderProcessed);
    }
}