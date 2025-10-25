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
        public OrderRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        /// <summary>
        /// Add a new order
        /// </summary>
        /// <param name="order"></param>
        public void AddOrder(Order order)
        {
            this.Context.Orders.Add(order);
            UnitOfWork.SaveChanges();
        }

        /// <summary>
        /// Update an existing order
        /// </summary>
        /// <param name="order"></param>
        public void UpdateOrder(Order order)
        {
            // Attach the entity and mark it as modified
            var existingOrder = this.Context.Orders.Find(order.OrderID);
            
            if (existingOrder != null)
            {
                // Update only the properties that are set
                this.Context.Entry(existingOrder).CurrentValues.SetValues(order);
                UnitOfWork.SaveChanges();
            }
            else
            {
                // If order doesn't exist in context, attach and update
                this.Context.Orders.Attach(order);
                this.Context.Entry(order).State = EntityState.Modified;
                UnitOfWork.SaveChanges();
            }
        }

        /// <summary>
        /// Delete an order by its ID
        /// </summary>
        /// <param name="orderID"></param>
        public void DeleteOrder(int orderID)
        {
            var order = this.Context.Orders.Find(orderID);
            if (order != null)
            {
                this.Context.Orders.Remove(order);
                UnitOfWork.SaveChanges();
            }
        }

        /// <summary>
        /// Get all orders
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Order> GetAllOrders()
        {
            if (this.Context == null)
            {
                throw new System.InvalidOperationException("Database context is not available.");
            }

            if (this.Context.Orders == null)
            {
                throw new System.InvalidOperationException("Orders DbSet is not available.");
            }

            return this.Context.Orders
                .Include(o => o.User)
                    .ThenInclude(u => u.UserProfile)
                .Include(o => o.Address)
                .Include(o => o.RestaurantPromotion)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.OrderItemOption)
                        .ThenInclude(oio => oio.ProductOptionGroup)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.OrderItemOption)
                        .ThenInclude(oio => oio.ProductOptionItems)
                .Include(o => o.OrderProcessed)
                .Include(o => o.PaymentLogs)
                .AsNoTracking() // Add this for performance
                .ToList();
        }

        public Order GetOrderById(int orderID)
        {
            return this.Context.Orders
                .Include(o => o.User)
                    .ThenInclude(u => u.UserProfile)
                .Include(o => o.Address)
                .Include(o => o.RestaurantPromotion)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.OrderItemOption)
                        .ThenInclude(oio => oio.ProductOptionGroup)  // ADD THIS LINE
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.OrderItemOption)
                        .ThenInclude(oio => oio.ProductOptionItems)  // ADD THIS LINE
                .Include(o => o.OrderProcessed)
                .Include(o => o.PaymentLogs)
                .FirstOrDefault(o => o.OrderID == orderID);
        }

        /// <summary>
        /// Check if a product is included in any order
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public bool IsProductInAnyOrder(int productId)
        {
            return this.Context.OrderItems
                .Any(oi => oi.ProductID == productId);
        }
    }
}