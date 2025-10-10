using ASI.Basecode.Data.Models;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IRestaurantProfile
    {
        Restaurant GetRestaurantProfile();
        void EditRestaurantProfile(Restaurant profile);
    }
}