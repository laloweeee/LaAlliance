using ASI.Basecode.Data.Models;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IRestaurantProfileRepository
    {
        RestaurantProfile GetRestaurantProfile();
        void EditRestaurantProfile(RestaurantProfile profile);
    }
}