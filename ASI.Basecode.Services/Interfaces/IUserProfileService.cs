using System.Collections.Generic;
using System.Linq;
using ASI.Basecode.Services.ServiceModels;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IUserProfileService
    {
        UserProfileServiceModel GetUserProfile(int userID);
        void UpdateUserProfile(UserProfileServiceModel userProfile);
    }
}