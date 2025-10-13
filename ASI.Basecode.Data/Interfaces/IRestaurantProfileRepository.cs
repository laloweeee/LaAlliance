using ASI.Basecode.Data.Models;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IRestaurantProfileRepository
    {
        Restaurant GetRestaurantProfile();
        void EditRestaurantProfile(Restaurant profile);
    }
}