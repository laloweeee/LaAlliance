using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.WebApp.Areas.Customer.Models;
using ASI.Basecode.WebApp.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;

namespace ASI.Basecode.WebApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    [AutoValidateAntiforgeryToken]
    public class FavoritesController : ControllerBase<FavoritesController>
    {
        private readonly IFavoriteService _favoriteService;

        public FavoritesController(
            IHttpContextAccessor httpContextAccessor,
            ILoggerFactory loggerFactory,
            IConfiguration configuration,
            IFavoriteService favoriteService) 
            : base(httpContextAccessor, loggerFactory, configuration, null, null)
        {
            _favoriteService = favoriteService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.UserName = User.FindFirst(ClaimTypes.Name)?.Value;
            ViewBag.FavoriteCount = _favoriteService.GetOrCreateFavorites(UserId).TotalItems;
            var favorites = _favoriteService.GetOrCreateFavorites(UserId);
            
            // Convert to ViewModel for the view
            var viewModel = new FavoriteViewModel
            {
                UserID = favorites.UserID,
                FavoriteItems = favorites.FavoriteItems.Select(fi => new FavoriteItemViewModel
                {
                    FavoriteItemID = fi.FavoriteItemID,
                    ProductID = fi.ProductID,
                    ProductImage = fi.ProductImage,
                    ProductName = fi.ProductName,
                    ProductPrice = fi.ProductPrice,
                    ProductDescription = fi.ProductDescription,
                    CategoryName = fi.CategoryName,
                    DateAdded = fi.DateAdded
                }).ToList()
            };
            
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(int productID)
        {
            try
            {
                _favoriteService.AddToFavorites(productID, UserId);
                TempData["SuccessMessage"] = "Item added to favorites!";
                return Json(new { success = true, message = "Added to favorites" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding to favorites");
                return Json(new { success = false, message = "Failed to add to favorites" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Remove(int favoriteItemID)
        {
            try
            {
                _favoriteService.RemoveFromFavorites(favoriteItemID, UserId);
                TempData["SuccessMessage"] = "Item removed from favorites!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing from favorites");
                TempData["ErrorMessage"] = "Failed to remove from favorites.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleFavorite(int productID) // This should work with both JSON and form data
        {
            try
            {
                var isInFavorites = _favoriteService.IsProductInFavorites(productID, UserId);
                
                if (isInFavorites)
                {
                    // Find the favorite item ID and remove it
                    var favorites = _favoriteService.GetOrCreateFavorites(UserId);
                    var favoriteItem = favorites.FavoriteItems.FirstOrDefault(fi => fi.ProductID == productID);
                    
                    if (favoriteItem != null)
                    {
                        _favoriteService.RemoveFromFavorites(favoriteItem.FavoriteItemID, UserId);
                        return Json(new { success = true, isFavorite = false, message = "Removed from favorites" });
                    }
                }
                else
                {
                    _favoriteService.AddToFavorites(productID, UserId);
                    return Json(new { success = true, isFavorite = true, message = "Added to favorites" });
                }

                return Json(new { success = false, message = "Operation failed" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling favorite");
                return Json(new { success = false, message = "Failed to update favorites" });
            }
        }

        [HttpGet]
        public IActionResult IsProductInFavorites(int productId)
        {
            try
            {
                var isFavorite = _favoriteService.IsProductInFavorites(productId, UserId);
                return Json(isFavorite);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking favorite status for product {ProductId}", productId);
                return Json(false);
            }
        }
    }
}