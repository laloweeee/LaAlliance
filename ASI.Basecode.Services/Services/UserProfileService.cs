using ASI.Basecode.Data.Interfaces;
using AutoMapper;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using System.Linq;
using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using ASI.Basecode.Services.Helper;
using CsvHelper;

namespace ASI.Basecode.Services.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IUserProfileRepository _userProfileRepository;

        public UserProfileService(IUserRepository userRepository, IUserProfileRepository userProfileRepository, IMapper mapper)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _userProfileRepository = userProfileRepository;
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

            var userProfile = _userProfileRepository.GetUserProfile(userID);

            ValidationHelper.ValidateNotNull(userProfile, nameof(userProfile));

            return _mapper.ProjectTo<UserProfileServiceModel>(userProfile).FirstOrDefault();
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

            var existingProfile = _userProfileRepository.GetUserProfile(userProfile.UserID).FirstOrDefault();

            ValidationHelper.ValidateNotNull(existingProfile, "User profile not found.");

            _mapper.Map(userProfile, existingProfile);
            _userProfileRepository.UpdateUserProfile(existingProfile);
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
        public void RemoveUserAddress(int userAddressID)
        {
            _userProfileRepository.RemoveUserAddress(userAddressID);
        }
    }
}