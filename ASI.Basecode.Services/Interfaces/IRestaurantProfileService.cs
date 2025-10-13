

using ASI.Basecode.Data.Models;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IRestaurantProfileService
    {
        Restaurant GetRestaurantProfile();
        void EditRestaurantInformation(Restaurant model);
        void EditRestaurantAddress(Address address);
    }
}