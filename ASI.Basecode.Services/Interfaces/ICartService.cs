using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.Data.Models;

namespace ASI.Basecode.Services.Interfaces
{
    public interface ICartService
    {
        CartViewModel GetOrCreateCart(int userID);
        CartItemViewModel GetCartItemForEdit(int cartItemID, int userID);
        void ClearCart(int userID);
        void RemoveCartItem(int cartItemID, int userID);
        void UpdateCartItemQuantity(int cartItemID, int quantity, int userID);
        void AddItemToCart(AddToCartRequest request, int userID);
    }
}