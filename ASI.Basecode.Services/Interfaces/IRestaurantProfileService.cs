using ASI.Basecode.Services.ServiceModels;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IRestaurantProfileService
    {
        RestaurantViewModel GetRestaurantProfile();
        void EditRestaurantInformation(RestaurantViewModel model);
        void EditRestaurantAddress(AddressViewModel address);
    }
}