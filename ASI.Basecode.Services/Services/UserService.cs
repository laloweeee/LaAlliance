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

        /// <summary>
        /// Authenticate user function
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public LoginResult AuthenticateUser(string email, string password, ref User user)
        {
            user = new User();

            var passwordKey = PasswordManager.EncryptPassword(password);
            user = _repository.GetUsers().Where(x => x.Email == email &&
                                                     x.Password == passwordKey).FirstOrDefault();

            return user != null ? LoginResult.Success : LoginResult.Failed;
        }

        /// <summary>
        /// Determines whether the specified user has the role of a customer.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool IsCustomer(User user)
        {
            return user?.UserType == UserType.Customer;
        }

        /// <summary>
        /// Determines whether the specified user has the role of a restaurant.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool IsRestaurant(User user)
        {
            return user?.UserType == UserType.Restaurant;
        }

        /// <summary>
        /// Add user function
        /// </summary>
        /// <param name="model"></param>
        /// <exception cref="InvalidDataException"></exception>
        public void AddUser(UserViewModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (_repository.UserExists(model.Email))
            {
                throw new InvalidDataException(Resources.Messages.Errors.UserExists);
            }

            var user = new User
            {
                Email = model.Email,
                UserProfile = new UserProfile
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName
                },
                Password = PasswordManager.EncryptPassword(model.Password),
                UserType = UserType.Customer,
                IsEmailVerified = false
            };

            _repository.AddUser(user);
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public User GetUserByID(int userID)
        {
            return _repository.GetUserByID(userID).FirstOrDefault();
        }
    }
}
