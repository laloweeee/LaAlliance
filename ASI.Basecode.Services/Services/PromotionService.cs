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

        /// <summary>
        /// Returns a mapping of ProductID -> applicable active promotion (if any).
        /// If multiple promotions apply to the same product, choose the one with higher DiscountValue.
        /// </summary>
        public System.Collections.Generic.IDictionary<int, RestaurantPromotions> GetActivePromotionProductMap()
        {
            var map = new System.Collections.Generic.Dictionary<int, RestaurantPromotions>();
            var active = GetActivePromotions() ?? new System.Collections.Generic.List<RestaurantPromotions>();

            foreach (var promo in active)
            {
                if (promo.PromotionProducts == null) continue;
                foreach (var pp in promo.PromotionProducts)
                {
                    if (pp?.ProductID == null) continue;
                    var pid = pp.ProductID.Value;
                    if (!map.ContainsKey(pid))
                    {
                        map[pid] = promo;
                    }
                    else
                    {
                        // prefer the promotion with higher DiscountValue
                        var existing = map[pid];
                        if (promo.DiscountValue > existing.DiscountValue)
                        {
                            map[pid] = promo;
                        }
                    }
                }
            }

            return map;
        }

        /// <summary>
        /// Validate a promotion code against the cart. Returns discount amount if valid.
        /// </summary>
        public ServiceModels.PromotionValidationResult ValidatePromotionCode(string code, ServiceModels.CartViewModel cart)
        {
            var result = new ServiceModels.PromotionValidationResult
            {
                IsValid = false,
                Message = "Invalid promotion code",
                DiscountAmount = 0
            };

            if (string.IsNullOrWhiteSpace(code))
            {
                result.Message = "Please provide a voucher code.";
                return result;
            }

            var norm = code.Trim().ToUpper();
            var promo = _promotionRepository.GetPromotionByCode(norm);
            if (promo == null)
            {
                result.Message = "Promotion code not found.";
                return result;
            }

            // find the exact code entry
            var promoCode = promo.PromotionCodes;
            if (promoCode == null || !string.Equals(promoCode.Code, norm, StringComparison.OrdinalIgnoreCase))
            {
                result.Message = "Promotion code not found.";
                return result;
            }

            var now = DateTime.Now;
            if (!promo.IsActive || promo.StartDate > now || promo.EndDate < now)
            {
                result.Message = "Promotion is not active.";
                return result;
            }

            if (promoCode.ExpirationDate != null && promoCode.ExpirationDate < now)
            {
                result.Message = "Promotion code has expired.";
                return result;
            }

            if (promo.MinimumOrderAmount > (cart?.SubTotal ?? 0))
            {
                result.Message = $"This promotion requires a minimum order of {promo.MinimumOrderAmount:C2}.";
                return result;
            }

            if (promoCode.UsageLimit > 0 && promoCode.UsedCount >= promoCode.UsageLimit)
            {
                result.Message = "This promotion code has reached its usage limit.";
                return result;
            }

            // compute applicable subtotal: if promo has product restrictions only apply to those products
            decimal applicable = 0m;
            if (promo.PromotionProducts != null && promo.PromotionProducts.Any())
            {
                var eligible = promo.PromotionProducts.Select(pp => pp.ProductID).ToHashSet();
                applicable = cart?.CartItems?.Where(ci => eligible.Contains(ci.ProductID)).Sum(ci => ci.TotalPrice) ?? 0m;
            }
            else
            {
                applicable = cart?.SubTotal ?? 0m;
            }

            if (applicable <= 0)
            {
                result.Message = "No items in the cart are eligible for this promotion.";
                return result;
            }

            decimal discount = 0m;
            // apply discount
            if (promo.DiscountType == Resources.Constants.Enums.DiscountType.Percentage)
            {
                discount = Math.Round(applicable * (promo.DiscountValue / 100m), 2);
            }
            else // Fixed amount
            {
                discount = Math.Min(applicable, promo.DiscountValue);
            }

            result.IsValid = true;
            result.Message = "Promotion applied successfully.";
            result.DiscountAmount = discount;
            result.PromotionID = promo.PromotionID;
            result.PromotionCodeID = promoCode.PromotionCodeID;

            return result;
        }

        /// <summary>
        /// Increment UsedCount for a promotion code (transactional at repository level).
        /// Throws if not found or usage limit reached.
        /// </summary>
        public void ConsumePromotionCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("Invalid code");
            var norm = code.Trim().ToUpper();

            // Use repository atomic consume to avoid race conditions
            var consumed = _promotionRepository.TryConsumePromotionCode(norm);
            if (!consumed)
            {
                throw new InvalidOperationException("Promotion code could not be consumed (may have reached usage limit).");
            }
        }
    }
}
