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
        /// <returns>The saved promotion with generated ID</returns>
        RestaurantPromotions AddPromotion(RestaurantPromotions promotion);

        /// <summary>
        /// Add a promotion code
        /// </summary>
        /// <param name="promotionCode"></param>
        void AddPromotionCode(PromotionCodes promotionCode);

        /// <summary>
        /// Add promotion products
        /// </summary>
        /// <param name="promotionProducts"></param>
        void AddPromotionProducts(IEnumerable<PromotionProducts> promotionProducts);

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

    /// <summary>
    /// Attempts to atomically consume (increment UsedCount) for a promotion code identified by the code string.
    /// Returns true when the UsedCount was incremented (i.e. usage limit not exceeded), false otherwise.
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    bool TryConsumePromotionCode(string code);
    }
}
