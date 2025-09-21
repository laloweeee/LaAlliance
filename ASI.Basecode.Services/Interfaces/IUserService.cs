using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.ServiceModels;
using System.Threading.Tasks;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IUserService
    {
        LoginResult AuthenticateUser(string email, string password, ref User user);
        Task<User> AuthenticateUserAsync(string email, string password);
        bool IsCustomer(User user);
        bool IsRestaurant(User user);
        // bool IsRestaurantAdmin(User user);
        // bool IsRestaurantStaff(User user);
        void AddUser(UserViewModel model);
    }
}
