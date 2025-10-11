using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace ASI.Basecode.Data.Repositories
{
    /// <summary>
    /// Repository for managing Cart entities
    /// </summary>
    public class CartRepository : BaseRepository, ICartRepository
    {
        private readonly AsiBasecodeDBContext _dbContext;

        public CartRepository(IUnitOfWork unitOfWork, AsiBasecodeDBContext dbContext) : base(unitOfWork)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Add a new cart
        /// </summary>
        /// <param name="cart"></param>
        public void AddCart(Cart cart)
        {
            _dbContext.Carts.Add(cart);
            UnitOfWork.SaveChanges();
        }

        /// <summary>
        /// Update an existing cart
        /// </summary>
        /// <param name="cart"></param>
        public void UpdateCart(Cart cart)
        {
            _dbContext.Carts.Update(cart);
            UnitOfWork.SaveChanges();
        }

        /// <summary>
        /// Delete a cart by its ID
        /// </summary>
        /// <param name="cartID"></param>
        public void DeleteCart(int cartID)
        {
            var cart = _dbContext.Carts.Find(cartID);
            if (cart != null)
            {
                _dbContext.Carts.Remove(cart);
                UnitOfWork.SaveChanges();
            }
        }

        /// <summary>
        /// Get a cart by its ID
        /// </summary>
        /// <param name="cartID"></param>
        /// <returns></returns>
        public Cart GetCartById(int cartID)
        {
            return _dbContext.Carts
                .Include(c => c.CartItem)
                    .ThenInclude(ci => ci.Product)
                .Include(c => c.CartItem)
                    .ThenInclude(ci => ci.CartItemOption)
                .Include(c => c.User)
                .FirstOrDefault(c => c.CartID == cartID);
        }

        /// <summary>
        /// Get cart by user ID
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public Cart GetCartByUserId(int userID)
        {
            return _dbContext.Carts
                .Include(c => c.CartItem)
                    .ThenInclude(ci => ci.Product)
                .Include(c => c.CartItem)
                    .ThenInclude(ci => ci.CartItemOption)
                .FirstOrDefault(c => c.UserID == userID);
        }

        /// <summary>
        /// Get all carts
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Cart> GetAllCarts()
        {
            return _dbContext.Carts
                .Include(c => c.User)
                .Include(c => c.CartItem)
                .ToList();
        }
    }
}
