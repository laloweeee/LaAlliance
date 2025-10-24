using ASI.Basecode.Data.Interfaces;
using AutoMapper;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using ASI.Basecode.Services.Helper;  // Ensure this is included for ValidationHelper

namespace ASI.Basecode.Services.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IAddressService _addressService;

        // Updated constructor to include all dependencies
        public UserProfileService(
            IMapper mapper,
            IUserRepository userRepository,
            IUserProfileRepository userProfileRepository,
            IAddressService addressService)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _userProfileRepository = userProfileRepository;
            _addressService = addressService;  // This is now included but not used in the methods below
        }

        /// <summary>
        /// Gets the user profile.
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public UserProfileServiceModel GetUserProfile(int userID)
        {
            ValidationHelper.ValidatePositiveInt(userID, "User ID");

            // Get user with profile data (includes email from User table)
            var user = _userRepository.GetUserByID(userID);
            ValidationHelper.ValidateNotNull(user, "User not found");

            // Create UserProfileServiceModel with data from both User and UserProfile tables
            var userProfileModel = new UserProfileServiceModel
            {
                UserID = user.UserID,
                Email = user.Email, // Email comes from User table
                FirstName = user.UserProfile?.FirstName ?? "",
                LastName = user.UserProfile?.LastName ?? "",
                ContactNumber = user.UserProfile?.ContactNumber ?? ""
            };

            return userProfileModel;
        }

        /// <summary>
        /// Updates the user profile.
        /// </summary>
        /// <param name="userProfile"></param>
        /// <exception cref="System.Exception"></exception>
        public void UpdateUserProfile(UserProfileServiceModel userProfile)
        {
            // Validate input
            ValidationHelper.ValidateNotNull(userProfile, nameof(userProfile));
            // Validate user ID
            ValidationHelper.ValidatePositiveInt(userProfile.UserID, "User ID");
            // Validate required fields
            ValidationHelper.ValidateRequired(userProfile.FirstName, "First Name");
            ValidationHelper.ValidateRequired(userProfile.LastName, "Last Name");
            // Validate email format
            ValidationHelper.ValidateEmail(userProfile.Email);

            // Get the user with profile data
            var user = _userRepository.GetUserByID(userProfile.UserID);
            ValidationHelper.ValidateNotNull(user, "User not found");

            // Update email in User table if it has changed
            if (user.Email != userProfile.Email)
            {
                user.Email = userProfile.Email;
                _userRepository.UpdateUser(user);
            }

            // Update UserProfile data
            if (user.UserProfile != null)
            {
                user.UserProfile.FirstName = userProfile.FirstName;
                user.UserProfile.LastName = userProfile.LastName;
                user.UserProfile.ContactNumber = userProfile.ContactNumber;
                _userProfileRepository.UpdateUserProfile(user.UserProfile);
            }
            else
            {
                // Create new UserProfile if it doesn't exist
                var newProfile = new UserProfile
                {
                    UserID = userProfile.UserID,
                    FirstName = userProfile.FirstName,
                    LastName = userProfile.LastName,
                    ContactNumber = userProfile.ContactNumber
                };
                _userProfileRepository.UpdateUserProfile(newProfile);
            }
        }

        /// <summary>
        /// Gets the user addresses.
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public List<UserAddressServiceModel> GetUserAddresses(int userID)
        {
            var userAddresses = _userProfileRepository.GetUserAddresses(userID);
            return _mapper.ProjectTo<UserAddressServiceModel>(userAddresses).ToList();
        }

        /// <summary>
        /// Adds the user address.
        /// </summary>
        /// <param name="userAddress"></param>
        public void AddUserAddress(UserAddressServiceModel userAddress)
        {
            var newUserAddress = _mapper.Map<UserAddress>(userAddress);
            _userProfileRepository.AddUserAddress(newUserAddress);
        }

        /// <summary>
        /// Updates the user address.
        /// </summary>
        /// <param name="userAddress"></param>
        /// <param name="userID"></param>
        /// <exception cref="System.Exception"></exception>
        public void UpdateUserAddress(UserAddressServiceModel userAddress, int userID)
        {
            ValidationHelper.ValidateNotNull(userAddress, nameof(userAddress));

            var existingAddress = _userProfileRepository.GetUserAddresses(userID)
                .FirstOrDefault(ua => ua.UserAddressID == userAddress.UserAddressID);

            ValidationHelper.ValidateNotNull(existingAddress, "User address not found.");
            
            _mapper.Map(userAddress, existingAddress);
            _userProfileRepository.UpdateUserAddress(existingAddress);
        }
        
        /// <summary>
        /// Removes the user address.
        /// </summary>
        /// <param name="userAddressID"></param>
        /// <param name="userID"></param>
        public void RemoveUserAddress(int userAddressID, int userID)
        {
            _userProfileRepository.RemoveUserAddress(userAddressID, userID);
        }
    }
}
