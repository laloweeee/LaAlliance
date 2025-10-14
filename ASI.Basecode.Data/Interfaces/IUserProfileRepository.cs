using System.Linq;
using ASI.Basecode.Data.Models;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IUserProfileRepository
    {
        // PROFILE RELATED
        IQueryable<UserProfile> GetUserProfile(int userID);
        void UpdateUserProfile(UserProfile userProfile);
        // ADDRESS RELATED
        IQueryable<UserAddress> GetUserAddresses(int userID);
        void AddUserAddress(UserAddress userAddress);
        void UpdateUserAddress(UserAddress userAddress);
        void RemoveUserAddress(int userAddressID);
    }
}