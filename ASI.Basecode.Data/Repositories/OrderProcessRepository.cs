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
            _dbContext.OrderProcesseds.Add(orderProcessed);
            _dbContext.SaveChanges();
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