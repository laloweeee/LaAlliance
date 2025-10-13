using ASI.Basecode.Data.Models;
using System.Linq;
using System.Collections.Generic;

namespace ASI.Basecode.Data.Interfaces
{
    public interface ICartRepository
    {
        // Cart operations
        IQueryable<Cart> GetCarts();
        Cart GetCartByUserID(int userId);
        void AddCart(Cart cart);
        void UpdateCart(Cart cart);
        void ClearCart(int userId);

        // CartItem operations
        IQueryable<CartItem> GetCartItems();
        void UpdateCartItem(CartItem cartItem);
        void AddCartItem(CartItem cartItem);
        CartItem GetCartItemByID(int cartItemId);
        void RemoveCartItem(CartItem cartItem);
        void AddCartItemOptions(int cartItemId, Dictionary<int, List<int>> selectedOptions);
    }
}