using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.ServiceModels;
using System.Linq;
using System.Threading.Tasks;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IUserService
    {
        LoginResult AuthenticateUser(string email, string password, ref User user);
        void AddUser(UserViewModel model);
        User GetUserByID(int userID);

        void UpdatePassword(int userID, string newPassword);
        bool VerifyPassword(int userID, string currentPassword);
    }
}
