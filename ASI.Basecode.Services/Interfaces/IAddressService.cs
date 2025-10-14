using ASI.Basecode.Services.ServiceModels;
using System.Collections.Generic;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IAddressService
    {
        List<UserAddressServiceModel> GetUserAddresses(int userID);
        void AddUserAddress(UserAddressServiceModel userAddress);
        void UpdateUserAddress(UserAddressServiceModel userAddress, int userID);
        void RemoveUserAddress(int userAddressID, int userID);
    }
}