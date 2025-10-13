using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using System;

namespace ASI.Basecode.Data.Repositories
{
    public class CartRepository : BaseRepository, ICartRepository
    {
        private readonly AsiBasecodeDBContext _dbContext;
        public CartRepository(IUnitOfWork unitOfWork, AsiBasecodeDBContext dbContext) : base(unitOfWork)
        {
            _dbContext = dbContext;
        }

        // CRUD FOR CART
        
        /// <summary>
        /// Get all carts with related items and options
        /// </summary>
        /// <returns></returns>
        public IQueryable<Cart> GetCarts()
        {
            return _dbContext.Carts
                .Include(c => c.CartItem)
                    .ThenInclude(ci => ci.Product)
                .Include(c => c.CartItem)
                    .ThenInclude(ci => ci.CartItemOption)
                        .ThenInclude(cio => cio.ProductOptionItems)
                .Include(c => c.CartItem)
                    .ThenInclude(ci => ci.CartItemOption)
                        .ThenInclude(cio => cio.ProductOptionGroup);
        }
        
        /// <summary>
        /// Get cart by user ID with related items and options
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public Cart GetCartByUserID(int userID)
        {
            return _dbContext.Carts
                .Include(c => c.CartItem)
                    .ThenInclude(ci => ci.Product)
                .Include(c => c.CartItem)
                    .ThenInclude(ci => ci.CartItemOption)
                        .ThenInclude(cio => cio.ProductOptionItems)
                .Include(c => c.CartItem)
                    .ThenInclude(ci => ci.CartItemOption)
                        .ThenInclude(cio => cio.ProductOptionGroup)
                .FirstOrDefault(c => c.UserID == userID);
        }
        
        /// <summary>
        /// Add new cart
        /// </summary>
        /// <param name="cart"></param>
        public void AddCart(Cart cart)
        {
            _dbContext.Carts.Add(cart);
            _dbContext.SaveChanges();
        }
        
        /// <summary>
        /// Update cart details
        /// </summary>
        /// <param name="cart"></param>
        public void UpdateCart(Cart cart)
        {
            _dbContext.Carts.Update(cart);
            _dbContext.SaveChanges();
        }
        
        /// <summary>
        /// Clear cart by removing it and all its items
        /// </summary>
        /// <param name="userId"></param>
        public void ClearCart(int userId)
        {
            var cart = GetCartByUserID(userId);
            if (cart != null)
            {
                _dbContext.Carts.Remove(cart);
                _dbContext.SaveChanges();
            }
        }

        // CRUD FOR CART ITEM

        /// <summary>
        /// Get all cart items with related product and options
        /// </summary>
        /// <returns></returns>
        public IQueryable<CartItem> GetCartItems()
        {
            return _dbContext.CartItems
                .Include(ci => ci.Cart)
                .Include(ci => ci.Product)
                .Include(ci => ci.CartItemOption)
                    .ThenInclude(cio => cio.ProductOptionItems)
                .Include(ci => ci.CartItemOption)
                    .ThenInclude(cio => cio.ProductOptionGroup)
                .AsNoTracking();
        }
        
        /// <summary>
        /// Get cart item by ID with related product and options
        /// </summary>
        /// <param name="cartItemId"></param>
        /// <returns></returns>
        public CartItem GetCartItemByID(int cartItemId)
        {
            return _dbContext.CartItems
                .Include(ci => ci.Cart)
                .Include(ci => ci.Product)
                .Include(ci => ci.CartItemOption)
                    .ThenInclude(cio => cio.ProductOptionItems)
                .Include(ci => ci.CartItemOption)
                    .ThenInclude(cio => cio.ProductOptionGroup)
                .FirstOrDefault(ci => ci.CartItemID == cartItemId);
        }

        /// <summary>
        /// Update cart item details
        /// </summary>
        /// <param name="cartItem"></param>
        public void UpdateCartItem(CartItem cartItem)
        {
            _dbContext.CartItems.Update(cartItem);
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Add new cart item
        /// </summary>
        /// <param name="cartItem"></param>
        public void AddCartItem(CartItem cartItem)
        {
            _dbContext.CartItems.Add(cartItem);
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Remove cart item and its options
        /// </summary>
        /// <param name="cartItem"></param>
        public void RemoveCartItem(CartItem cartItem)
        {
            var item = _dbContext.CartItems
                .Include(ci => ci.CartItemOption)
                .FirstOrDefault(ci => ci.CartItemID == cartItem.CartItemID);

            if (item != null)
            {
                _dbContext.CartItems.Remove(cartItem);
                _dbContext.SaveChanges();
            }
        }

        /// <summary>
        /// Add options to a cart item
        /// </summary>
        /// <param name="cartItemId"></param>
        /// <param name="selectedOptions"></param>
        public void AddCartItemOptions(int cartItemId, Dictionary<int, List<int>> selectedOptions)
        {
            if (selectedOptions == null || !selectedOptions.Any())
                return;

            var optionsToAdd = new List<CartItemOption>();

            foreach (var optionGroup in selectedOptions)
            {
                int groupId = optionGroup.Key;
                foreach (var optionItemId in optionGroup.Value)
                {
                    var cartItemOption = new CartItemOption
                    {
                        CartItemID = cartItemId,
                        ProductOptionGroupID = groupId,
                        ProductOptionItemID = optionItemId
                    };

                    optionsToAdd.Add(cartItemOption);
                }
            }

            if (optionsToAdd.Any())
            {
                _dbContext.CartItemOptions.AddRange(optionsToAdd);
                _dbContext.SaveChanges();
            }
        }
    }
}