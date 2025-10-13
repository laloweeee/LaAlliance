using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASI.Basecode.Data.Repositories
{
    /// <summary>
    /// Repository for managing RestaurantPromotions entities
    /// </summary>
    public class PromotionRepository : BaseRepository, IPromotionRepository
    {
        private readonly AsiBasecodeDBContext _dbContext;

        public PromotionRepository(IUnitOfWork unitOfWork, AsiBasecodeDBContext dbContext) : base(unitOfWork)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Add a new promotion
        /// </summary>
        /// <param name="promotion"></param>
        public void AddPromotion(RestaurantPromotions promotion)
        {
            _dbContext.RestaurantPromotions.Add(promotion);
            UnitOfWork.SaveChanges();
        }

        /// <summary>
        /// Update an existing promotion
        /// </summary>
        /// <param name="promotion"></param>
        public void UpdatePromotion(RestaurantPromotions promotion)
        {
            _dbContext.RestaurantPromotions.Update(promotion);
            UnitOfWork.SaveChanges();
        }

        /// <summary>
        /// Delete a promotion by its ID
        /// </summary>
        /// <param name="promotionID"></param>
        public void DeletePromotion(int promotionID)
        {
            var promotion = _dbContext.RestaurantPromotions.Find(promotionID);
            if (promotion != null)
            {
                _dbContext.RestaurantPromotions.Remove(promotion);
                UnitOfWork.SaveChanges();
            }
        }

        /// <summary>
        /// Get a promotion by its ID
        /// </summary>
        /// <param name="promotionID"></param>
        /// <returns></returns>
        public RestaurantPromotions GetPromotionById(int promotionID)
        {
            return _dbContext.RestaurantPromotions
                .Include(p => p.PromotionCodes)
                .Include(p => p.PromotionProducts)
                    .ThenInclude(pp => pp.Product)
                .Include(p => p.Order)
                .FirstOrDefault(p => p.PromotionID == promotionID);
        }

        /// <summary>
        /// Get all promotions
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RestaurantPromotions> GetAllPromotions()
        {
            return _dbContext.RestaurantPromotions
                .Include(p => p.PromotionCodes)
                .Include(p => p.PromotionProducts)
                .ToList();
        }

        /// <summary>
        /// Get active promotions
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RestaurantPromotions> GetActivePromotions()
        {
            var currentDate = DateTime.Now;
            return _dbContext.RestaurantPromotions
                .Include(p => p.PromotionCodes)
                .Include(p => p.PromotionProducts)
                .Where(p => p.IsActive && p.StartDate <= currentDate && p.EndDate >= currentDate)
                .ToList();
        }

        /// <summary>
        /// Get promotions by date range
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public IEnumerable<RestaurantPromotions> GetPromotionsByDate(DateTime date)
        {
            return _dbContext.RestaurantPromotions
                .Include(p => p.PromotionCodes)
                .Include(p => p.PromotionProducts)
                .Where(p => p.StartDate <= date && p.EndDate >= date)
                .ToList();
        }

        /// <summary>
        /// Get promotion by code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public RestaurantPromotions GetPromotionByCode(string code)
        {
            return _dbContext.RestaurantPromotions
                .Include(p => p.PromotionCodes)
                .Include(p => p.PromotionProducts)
                    .ThenInclude(pp => pp.Product)
                .FirstOrDefault(p => p.PromotionCodes.Code == code);
        }
    }
}
