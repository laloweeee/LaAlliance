using System.Collections.Generic;
using System.Linq;
using ASI.Basecode.Services.ServiceModels;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IUserProfileService
    {
        UserProfileServiceModel GetUserProfile(int userID);
        void UpdateUserProfile(UserProfileServiceModel userProfile);
        List<UserAddressServiceModel> GetUserAddresses(int userID);
        void AddUserAddress(UserAddressServiceModel userAddress);
        void UpdateUserAddress(UserAddressServiceModel userAddress, int userID);
        void RemoveUserAddress(int userAddressID);
    }
}