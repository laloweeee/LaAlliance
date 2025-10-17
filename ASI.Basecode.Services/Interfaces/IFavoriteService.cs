using ASI.Basecode.Services.ServiceModels;
using System.Collections.Generic;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IFavoriteService
    {
        FavoriteServiceModel GetOrCreateFavorites(int userId);
        void AddToFavorites(int productId, int userId);
        void RemoveFromFavorites(int favoriteItemId, int userId);
        bool IsProductInFavorites(int productId, int userId);
        FavoriteItemServiceModel GetFavoriteItem(int favoriteItemId, int userId);
        List<FavoriteItemServiceModel> GetUserFavorites(int userId);
    }
}