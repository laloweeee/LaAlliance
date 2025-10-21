using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Services.Services
{
    /// <summary>
    /// Service for managing promotions.
    /// </summary>
    public class PromotionService : ServiceBase, IPromotionService
    {
        private readonly IPromotionRepository _promotionRepository;
        private readonly IFileHandlingService _fileHandlingService;

        /// <summary>
        /// Constructor for PromotionService.
        /// </summary>
        /// <param name="promotionRepository"></param>
        /// <param name="fileHandlingService"></param>
        /// <param name="loggerFactory"></param>
        public PromotionService(
            IPromotionRepository promotionRepository,
            IFileHandlingService fileHandlingService,
            ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            _promotionRepository = promotionRepository;
            _fileHandlingService = fileHandlingService;
        }

        /// <summary>
        /// Add a new promotion.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task AddPromotion(PromotionViewModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            // Validate date range
            if (model.StartDate >= model.EndDate)
            {
                throw new ArgumentException("Start date must be before end date.");
            }

            // Check if promotion name already exists
            var existingPromotion = _promotionRepository.GetAllPromotions()
                .FirstOrDefault(p => p.PromotionName.ToLower().Trim() == model.PromotionName.ToLower().Trim());

            if (existingPromotion != null)
            {
                throw new ArgumentException("A promotion with this name already exists.");
            }

            var promotion = new RestaurantPromotions
            {
                PromotionName = model.PromotionName.Trim(),
                PromotionDescription = model.PromotionDescription?.Trim(),
                DiscountType = model.DiscountType,
                DiscountValue = model.DiscountValue,
                MinimumOrderAmount = model.MinimumOrderAmount,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                IsActive = model.IsActive
            };

            // Handle image upload if provided
            if (model.PromotionalBannerFile != null)
            {
                var imageUrl = await _fileHandlingService.HandleFile(model.PromotionalBannerFile, "promotions");
                promotion.PromotionBanner = imageUrl;
            }

            // Add promotion to database
            var savedPromotion = _promotionRepository.AddPromotion(promotion);

            // Add promotion code if provided
            if (!string.IsNullOrEmpty(model.Code))
            {
                var promotionCode = new PromotionCodes
                {
                    PromotionID = savedPromotion.PromotionID,
                    Code = model.Code.Trim().ToUpper(),
                    UsageLimit = model.UsageLimit,
                    UsedCount = model.UsedCount,
                    ExpirationDate = model.ExpirationDate
                };

                _promotionRepository.AddPromotionCode(promotionCode);
            }

            // Add associated products if provided
            if (model.ProductIDs != null && model.ProductIDs.Count > 0)
            {
                var promotionProducts = model.ProductIDs.Select(productId => new PromotionProducts
                {
                    PromotionID = savedPromotion.PromotionID,
                    ProductID = productId
                }).ToList();

                _promotionRepository.AddPromotionProducts(promotionProducts);
            }

            _logger.LogInformation($"Promotion '{model.PromotionName}' created successfully with ID: {savedPromotion.PromotionID}");
        }

        /// <summary>
        /// Update an existing promotion.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task UpdatePromotion(PromotionViewModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var existingPromotion = _promotionRepository.GetPromotionById(model.PromotionID);
            if (existingPromotion == null)
            {
                throw new ArgumentException("Promotion not found.");
            }

            // Validate date range
            if (model.StartDate >= model.EndDate)
            {
                throw new ArgumentException("Start date must be before end date.");
            }

            // Check if promotion name already exists (excluding current promotion)
            var duplicatePromotion = _promotionRepository.GetAllPromotions()
                .FirstOrDefault(p => p.PromotionName.ToLower().Trim() == model.PromotionName.ToLower().Trim() 
                                   && p.PromotionID != model.PromotionID);

            if (duplicatePromotion != null)
            {
                throw new ArgumentException("A promotion with this name already exists.");
            }

            // Update promotion properties
            existingPromotion.PromotionName = model.PromotionName.Trim();
            existingPromotion.PromotionDescription = model.PromotionDescription?.Trim();
            existingPromotion.DiscountType = model.DiscountType;
            existingPromotion.DiscountValue = model.DiscountValue;
            existingPromotion.MinimumOrderAmount = model.MinimumOrderAmount;
            existingPromotion.StartDate = model.StartDate;
            existingPromotion.EndDate = model.EndDate;
            existingPromotion.IsActive = model.IsActive;

            // Handle image upload if provided
            if (model.PromotionalBannerFile != null)
            {
                var imageUrl = await _fileHandlingService.HandleFile(model.PromotionalBannerFile, "promotions");
                existingPromotion.PromotionBanner = imageUrl;
            }

            _promotionRepository.UpdatePromotion(existingPromotion);

            _logger.LogInformation($"Promotion '{model.PromotionName}' updated successfully with ID: {model.PromotionID}");
        }

        /// <summary>
        /// Delete a promotion by its ID.
        /// </summary>
        /// <param name="promotionID"></param>
        public void DeletePromotion(int promotionID)
        {
            var promotion = _promotionRepository.GetPromotionById(promotionID);
            if (promotion == null)
            {
                throw new ArgumentException("Promotion not found.");
            }

            _promotionRepository.DeletePromotion(promotionID);
            _logger.LogInformation($"Promotion '{promotion.PromotionName}' deleted successfully with ID: {promotionID}");
        }

        /// <summary>
        /// Get a promotion by its ID.
        /// </summary>
        /// <param name="promotionID"></param>
        /// <returns></returns>
        public RestaurantPromotions GetPromotionById(int promotionID)
        {
            return _promotionRepository.GetPromotionById(promotionID);
        }

        /// <summary>
        /// Get all promotions.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RestaurantPromotions> GetAllPromotions()
        {
            return _promotionRepository.GetAllPromotions();
        }

        /// <summary>
        /// Get active promotions.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<RestaurantPromotions> GetActivePromotions()
        {
            return _promotionRepository.GetActivePromotions();
        }

        /// <summary>
        /// Get promotions by date range.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public IEnumerable<RestaurantPromotions> GetPromotionsByDate(DateTime date)
        {
            return _promotionRepository.GetPromotionsByDate(date);
        }

        /// <summary>
        /// Get promotion by code.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public RestaurantPromotions GetPromotionByCode(string code)
        {
            return _promotionRepository.GetPromotionByCode(code);
        }
    }
}
