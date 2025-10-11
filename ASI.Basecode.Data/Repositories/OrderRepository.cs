using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using static ASI.Basecode.Resources.Constants.Enums;

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
        /// Get an order by its ID
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        public Order GetOrderById(int orderID)
        {
            return _dbContext.Orders
                .Include(o => o.User)
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

        /// <summary>
        /// Get all orders for a specific user
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public IEnumerable<Order> GetOrdersByUserId(int userID)
        {
            return _dbContext.Orders
                .Include(o => o.Address)
                .Include(o => o.RestaurantPromotion)
                .Include(o => o.OrderItems)
                .Include(o => o.OrderProcessed)
                .Where(o => o.UserID == userID)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }

        /// <summary>
        /// Get all orders
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Order> GetAllOrders()
        {
            return _dbContext.Orders
                .Include(o => o.User)
                .Include(o => o.Address)
                .Include(o => o.OrderItems)
                .Include(o => o.OrderProcessed)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }

        /// <summary>
        /// Get orders by status
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public IEnumerable<Order> GetOrdersByStatus(OrderStatus status)
        {
            return _dbContext.Orders
                .Include(o => o.User)
                .Include(o => o.Address)
                .Include(o => o.OrderItems)
                .Where(o => o.OrderStatus == status)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }

        /// <summary>
        /// Get orders by date range
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public IEnumerable<Order> GetOrdersByDateRange(DateTime startDate, DateTime endDate)
        {
            return _dbContext.Orders
                .Include(o => o.User)
                .Include(o => o.Address)
                .Include(o => o.OrderItems)
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }
    }
}
