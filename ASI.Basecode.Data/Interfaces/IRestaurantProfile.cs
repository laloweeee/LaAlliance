using ASI.Basecode.Data.Models;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IRestaurantProfile
    {
        RestaurantProfile GetRestaurantProfile();
        void EditRestaurantProfile(RestaurantProfile profile);
    }
}