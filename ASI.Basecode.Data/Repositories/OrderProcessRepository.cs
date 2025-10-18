using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Data.Repositories
{
    public class OrderProcessedRepository : BaseRepository, IOrderProcessedRepository
    {
        private readonly AsiBasecodeDBContext _dbContext;
        public OrderProcessedRepository(IUnitOfWork unitOfWork, AsiBasecodeDBContext dbContext) : base(unitOfWork)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Get all OrderProcesseds
        /// </summary>
        /// <returns></returns>
        public IEnumerable<OrderProcessed> GetAllOrderProcesseds()
        {
            return _dbContext.OrderProcesseds.ToList();
        }
        /// <summary>
        /// Get OrderProcessed by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public OrderProcessed GetOrderProcessedById(int id)
        {
            return _dbContext.OrderProcesseds.Find(id);
        }

        /// <summary>
        /// Add a new OrderProcessed
        /// </summary>
        /// <param name="orderProcessed"></param>
        public void AddOrderProcessed(OrderProcessed orderProcessed)
        {
            Console.WriteLine($"→ Repository: Adding OrderProcessed for Order {orderProcessed.OrderID}");
            
            try
            {
                _dbContext.OrderProcesseds.Add(orderProcessed);
                Console.WriteLine($"→ Repository: Entity added to context");
                
                _dbContext.SaveChanges();
                Console.WriteLine($"✓ Repository: Changes saved to database");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗✗✗ Repository ERROR ✗✗✗");
                Console.WriteLine($"Error Message: {ex.Message}");
                Console.WriteLine($"Error Type: {ex.GetType().Name}");
                
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                
                throw; // Re-throw to be caught by the service layer
            }
        }

        /// <summary>
        /// Update an existing OrderProcessed
        /// </summary>
        /// <param name="orderProcessed"></param>
        public void UpdateOrderProcessed(OrderProcessed orderProcessed)
        {
            _dbContext.OrderProcesseds.Update(orderProcessed);
            _dbContext.SaveChanges();
        }
    }
}