

using ASI.Basecode.Data.Models;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IRestaurantProfileService
    {
        RestaurantProfile GetRestaurantProfile();
        void EditRestaurantInformation(RestaurantProfile model, int userID);
        void EditRestaurantAddress(Address address, int userID);
    }
}