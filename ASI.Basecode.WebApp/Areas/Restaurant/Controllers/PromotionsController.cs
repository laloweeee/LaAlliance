using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Basecode.WebApp.Areas.Restaurant.Controllers
{
    [Authorize(Policy = "RestaurantAdmin")]
    [Area("Restaurant")]
    public class PromotionsController : ControllerBase<PromotionsController>
    {
        private readonly IProductService _productService;
        private readonly IPromotionService _promotionService;

        public PromotionsController(
                                        IHttpContextAccessor httpContextAccessor,
                                        ILoggerFactory loggerFactory,
                                        IConfiguration configuration,
                                        IMapper mapper,
                                        IProductService productService,
                                        IPromotionService promotionService
                                    ) : base(httpContextAccessor, loggerFactory, configuration, mapper) 
        {
            _productService = productService;
            _promotionService = promotionService;
        }

        public IActionResult Index()
        {
            var promotions = _promotionService.GetAllPromotions();
            var products = _productService.GetActiveProducts();
            ViewBag.Products = products;
            return View(promotions);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePromotion(PromotionViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _promotionService.AddPromotion(model);
                    TempData["SuccessMessage"] = "Promotion created successfully!";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while creating the promotion. Please try again.");
                _logger.LogError(ex, "Error creating promotion");
            }

            var products = _productService.GetActiveProducts();
            ViewBag.Products = products;
            return View(model);
        }

        public IActionResult Edit(int id)
        {
            var promotion = _promotionService.GetPromotionById(id);
            if (promotion == null)
            {
                return NotFound();
            }

            var model = new PromotionViewModel
            {
                PromotionID = promotion.PromotionID,
                PromotionName = promotion.PromotionName,
                PromotionBanner = promotion.PromotionBanner,
                PromotionDescription = promotion.PromotionDescription,
                DiscountType = promotion.DiscountType,
                DiscountValue = promotion.DiscountValue,
                MinimumOrderAmount = promotion.MinimumOrderAmount,
                StartDate = promotion.StartDate,
                EndDate = promotion.EndDate,
                IsActive = promotion.IsActive,
                ProductIDs = promotion.PromotionProducts?.Select(pp => pp.ProductID ?? 0).ToList() ?? new List<int>()
            };

            if (promotion.PromotionCodes != null)
            {
                model.Code = promotion.PromotionCodes.Code;
                model.UsageLimit = promotion.PromotionCodes.UsageLimit;
                model.UsedCount = promotion.PromotionCodes.UsedCount;
                model.ExpirationDate = promotion.PromotionCodes.ExpirationDate;
            }

            var products = _productService.GetActiveProducts();
            ViewBag.Products = products;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PromotionViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _promotionService.UpdatePromotion(model);
                    TempData["SuccessMessage"] = "Promotion updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while updating the promotion. Please try again.");
                _logger.LogError(ex, "Error updating promotion");
            }

            var products = _productService.GetActiveProducts();
            ViewBag.Products = products;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                _promotionService.DeletePromotion(id);
                TempData["SuccessMessage"] = "Promotion deleted successfully!";
            }
            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the promotion. Please try again.";
                _logger.LogError(ex, "Error deleting promotion");
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult GetActivePromotions()
        {
            var promotions = _promotionService.GetActivePromotions();
            return Json(promotions);
        }
    }
}
