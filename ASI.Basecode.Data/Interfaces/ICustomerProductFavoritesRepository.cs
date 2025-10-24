using ASI.Basecode.Data.Models;
using System.Collections.Generic;
using System.Linq;

namespace ASI.Basecode.Data.Interfaces
{
    public interface ICustomerProductFavoritesRepository
    {
        /// <summary>
        /// Get all customer product favorites
        /// </summary>
        /// <returns></returns>
        IQueryable<CustomerProductFavorites> GetCustomerProductFavorites();
        
        /// <summary>
        /// Get favorite items count by product
        /// </summary>
        /// <returns></returns>
        Dictionary<int, int> GetFavoriteCountsByProduct();
        
        /// <summary>
        /// Add product to customer favorites
        /// </summary>
        /// <param name="favorite"></param>
        void AddFavorite(CustomerProductFavorites favorite);
        
        /// <summary>
        /// Remove product from customer favorites
        /// </summary>
        /// <param name="favorite"></param>
        void RemoveFavorite(CustomerProductFavorites favorite);
        
        /// <summary>
        /// Check if product is favorited by user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        bool IsProductFavoritedByUser(int userId, int productId);
        
        /// <summary>
        /// Get customer's favorite products
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        IQueryable<CustomerProductFavorites> GetUserFavorites(int userId);
    }
}