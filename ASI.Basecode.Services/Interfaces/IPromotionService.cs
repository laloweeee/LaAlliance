using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Interfaces
{
    /// <summary>
    /// Interface for Promotion Service
    /// </summary>
    public interface IPromotionService
    {
        /// <summary>
        /// Add a new promotion
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task AddPromotion(PromotionViewModel model);

        /// <summary>
        /// Update an existing promotion
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task UpdatePromotion(PromotionViewModel model);

        /// <summary>
        /// Delete a promotion by its ID
        /// </summary>
        /// <param name="promotionID"></param>
        void DeletePromotion(int promotionID);

        /// <summary>
        /// Get a promotion by its ID
        /// </summary>
        /// <param name="promotionID"></param>
        /// <returns></returns>
        RestaurantPromotions GetPromotionById(int promotionID);

        /// <summary>
        /// Get all promotions
        /// </summary>
        /// <returns></returns>
        IEnumerable<RestaurantPromotions> GetAllPromotions();

        /// <summary>
        /// Get active promotions
        /// </summary>
        /// <returns></returns>
        IEnumerable<RestaurantPromotions> GetActivePromotions();

        /// <summary>
        /// Get promotions by date range
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        IEnumerable<RestaurantPromotions> GetPromotionsByDate(DateTime date);

        /// <summary>
        /// Get promotion by code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        RestaurantPromotions GetPromotionByCode(string code);
    }
}
