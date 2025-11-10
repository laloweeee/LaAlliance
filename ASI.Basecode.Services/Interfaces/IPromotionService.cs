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

    /// <summary>
    /// Returns a mapping of ProductID -> applicable active promotion (if any).
    /// If multiple promotions apply to the same product, the one with higher DiscountValue is chosen.
    /// </summary>
    /// <returns></returns>
    System.Collections.Generic.IDictionary<int, RestaurantPromotions> GetActivePromotionProductMap();

    /// <summary>
    /// Validate a promotion code against the user's cart. Returns discount amount and message.
    /// </summary>
    /// <param name="code"></param>
    /// <param name="cart"></param>
    /// <returns></returns>
    ServiceModels.PromotionValidationResult ValidatePromotionCode(string code, ServiceModels.CartViewModel cart);

    /// <summary>
    /// Consume (increment UsedCount) for a promotion code. Throws if invalid or usage limit reached.
    /// </summary>
    /// <param name="code"></param>
    void ConsumePromotionCode(string code);
    }
}
