using ASI.Basecode.Data.Models;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Interfaces
{
    /// <summary>
    /// Interface for Cart Repository
    /// </summary>
    public interface ICartRepository
    {
        /// <summary>
        /// Add a new cart
        /// </summary>
        /// <param name="cart"></param>
        void AddCart(Cart cart);

        /// <summary>
        /// Update an existing cart
        /// </summary>
        /// <param name="cart"></param>
        void UpdateCart(Cart cart);

        /// <summary>
        /// Delete a cart by its ID
        /// </summary>
        /// <param name="cartID"></param>
        void DeleteCart(int cartID);

        /// <summary>
        /// Get a cart by its ID
        /// </summary>
        /// <param name="cartID"></param>
        /// <returns></returns>
        Cart GetCartById(int cartID);

        /// <summary>
        /// Get cart by user ID
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        Cart GetCartByUserId(int userID);

        /// <summary>
        /// Get all carts
        /// </summary>
        /// <returns></returns>
        IEnumerable<Cart> GetAllCarts();
    }
}
