using ASI.Basecode.Data;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASI.Basecode.Services.Services
{
    public class FavoriteService : IFavoriteService // FIXED: removed extra 'm'
    {
        private readonly AsiBasecodeDBContext _context;
        private readonly ILogger<FavoriteService> _logger;

        public FavoriteService(AsiBasecodeDBContext context, ILogger<FavoriteService> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ... rest of your service methods remain the same
        public FavoriteServiceModel GetOrCreateFavorites(int userId)
        {
            try
            {
                var favoriteItems = _context.CustomerProductFavorites
                    .Include(f => f.Product)
                    .ThenInclude(p => p.ProductCategory)
                    .Where(f => f.UserID == userId && f.Product.IsActive)
                    .ToList();

                return new FavoriteServiceModel
                {
                    UserID = userId,
                    FavoriteItems = favoriteItems.Select(MapToItemServiceModel).ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting favorites for user {UserId}", userId);
                throw;
            }
        }

        public void AddToFavorites(int productId, int userId)
        {
            try
            {
                var existingFavorite = _context.CustomerProductFavorites
                    .FirstOrDefault(f => f.UserID == userId && f.ProductID == productId);

                if (existingFavorite == null)
                {
                    var favorite = new CustomerProductFavorites
                    {
                        UserID = userId,
                        ProductID = productId,
                        DateAdded = DateTime.Now
                    };
                    _context.CustomerProductFavorites.Add(favorite);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding product {ProductId} to favorites for user {UserId}", productId, userId);
                throw;
            }
        }

        public void RemoveFromFavorites(int favoriteItemId, int userId)
        {
            try
            {
                var favoriteItem = _context.CustomerProductFavorites
                    .FirstOrDefault(f => f.CustomerProductFavoriteID == favoriteItemId && f.UserID == userId);

                if (favoriteItem != null)
                {
                    _context.CustomerProductFavorites.Remove(favoriteItem);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing favorite item {FavoriteItemId} for user {UserId}", favoriteItemId, userId);
                throw;
            }
        }

        public bool IsProductInFavorites(int productId, int userId)
        {
            return _context.CustomerProductFavorites
                .Any(f => f.ProductID == productId && f.UserID == userId);
        }

        public FavoriteItemServiceModel GetFavoriteItem(int favoriteItemId, int userId)
        {
            var favoriteItem = _context.CustomerProductFavorites
                .Include(f => f.Product)
                .ThenInclude(p => p.ProductCategory)
                .FirstOrDefault(f => f.CustomerProductFavoriteID == favoriteItemId && f.UserID == userId);

            return favoriteItem != null ? MapToItemServiceModel(favoriteItem) : null;
        }

        public List<FavoriteItemServiceModel> GetUserFavorites(int userId)
        {
            var favoriteItems = _context.CustomerProductFavorites
                .Include(f => f.Product)
                .ThenInclude(p => p.ProductCategory)
                .Where(f => f.UserID == userId && f.Product.IsActive)
                .OrderByDescending(f => f.DateAdded)
                .ToList();

            return favoriteItems.Select(MapToItemServiceModel).ToList();
        }

        private FavoriteItemServiceModel MapToItemServiceModel(CustomerProductFavorites favoriteItem)
        {
            return new FavoriteItemServiceModel
            {
                FavoriteItemID = favoriteItem.CustomerProductFavoriteID,
                ProductID = favoriteItem.ProductID,
                ProductImage = favoriteItem.Product?.ProductImage,
                ProductName = favoriteItem.Product?.ProductName,
                ProductPrice = favoriteItem.Product?.ProductPrice ?? 0,
                ProductDescription = favoriteItem.Product?.ProductDescription,
                CategoryName = favoriteItem.Product?.ProductCategory?.CategoryName ?? "Uncategorized",
                DateAdded = favoriteItem.DateAdded
            };
        }
    }
}