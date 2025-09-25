using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.Manager;
using ASI.Basecode.Services.ServiceModels;
using AutoMapper;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository repository, IMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public LoginResult AuthenticateUser(string email, string password, ref User user)
        {
            user = new User();

            var passwordKey = PasswordManager.EncryptPassword(password);
            user = _repository.GetUsers().Where(x => x.Email == email &&
                                                     x.Password == passwordKey).FirstOrDefault();

            return user != null ? LoginResult.Success : LoginResult.Failed;
        }

        public bool IsCustomer(User user)
        {
            return user?.Role == UserRole.Customer.ToString();
        }

        /// <summary>
        /// Determines whether the specified user has the role of a restaurant.
        /// </summary>
        /// <param name="user">The user to evaluate. Must not be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if the user's role is "Restaurant"; otherwise, <see langword="false"/>.</returns>
        public bool IsRestaurant(User user)
        {
            return user?.Role == UserRole.Restaurant.ToString();
        }

        /// <summary>
        /// Add user function
        /// </summary>
        /// <param name="model"></param>
        /// <exception cref="InvalidDataException"></exception>
        public void AddUser(UserViewModel model)
        {
            var user = new User();
            if (!_repository.UserExists(model.Email))
            {
                _mapper.Map(model, user);
                user.Email = model.Email;
                user.Password = PasswordManager.EncryptPassword(model.Password);
                user.Role = UserRole.Customer.ToString();
                user.CreatedTime = DateTime.Now;
                user.UpdatedTime = DateTime.Now;

                _repository.AddUser(user);
            }
            else
            {
                throw new InvalidDataException(Resources.Messages.Errors.UserExists);
            }
        }
    }
}
