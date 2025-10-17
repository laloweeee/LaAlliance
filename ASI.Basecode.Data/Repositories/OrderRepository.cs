using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace ASI.Basecode.Data.Repositories
{
    /// <summary>
    /// Repository for managing Order entities
    /// </summary>
    public class OrderRepository : BaseRepository, IOrderRepository
    {
        private readonly AsiBasecodeDBContext _dbContext;

        public OrderRepository(IUnitOfWork unitOfWork, AsiBasecodeDBContext dbContext) : base(unitOfWork)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Add a new order
        /// </summary>
        /// <param name="order"></param>
        public void AddOrder(Order order)
        {
            _dbContext.Orders.Add(order);
            UnitOfWork.SaveChanges();
        }

        /// <summary>
        /// Update an existing order
        /// </summary>
        /// <param name="order"></param>
        public void UpdateOrder(Order order)
        {
            _dbContext.Orders.Update(order);
            UnitOfWork.SaveChanges();
        }

        /// <summary>
        /// Delete an order by its ID
        /// </summary>
        /// <param name="orderID"></param>
        public void DeleteOrder(int orderID)
        {
            var order = _dbContext.Orders.Find(orderID);
            if (order != null)
            {
                _dbContext.Orders.Remove(order);
                UnitOfWork.SaveChanges();
            }
        }

        /// <summary>
        /// Get all orders
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Order> GetAllOrders()
        {
            return _dbContext.Orders
                .Include(o => o.User)
                    .ThenInclude(u => u.UserProfile)
                .Include(o => o.Address)
                .Include(o => o.RestaurantPromotion)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.OrderItemOption)
                .Include(o => o.OrderProcessed)
                .Include(o => o.PaymentLogs).ToList();
        }

        public Order GetOrderById(int orderID)
        {
            return _dbContext.Orders
                .Include(o => o.User)
                    .ThenInclude(u => u.UserProfile)
                .Include(o => o.Address)
                .Include(o => o.RestaurantPromotion)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.OrderItemOption)
                .Include(o => o.OrderProcessed)
                .Include(o => o.PaymentLogs)
                .FirstOrDefault(o => o.OrderID == orderID);
        }
    }
}
