using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Interfaces
{
    /// <summary>
    /// Interface for Promotion Repository
    /// </summary>
    public interface IPromotionRepository
    {
        /// <summary>
        /// Add a new promotion
        /// </summary>
        /// <param name="promotion"></param>
        void AddPromotion(RestaurantPromotions promotion);

        /// <summary>
        /// Update an existing promotion
        /// </summary>
        /// <param name="promotion"></param>
        void UpdatePromotion(RestaurantPromotions promotion);

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
