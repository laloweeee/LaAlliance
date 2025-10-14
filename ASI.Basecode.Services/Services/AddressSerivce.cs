using ASI.Basecode.Data.Interfaces;
using AutoMapper;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using System.Linq;
using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using ASI.Basecode.Services.Helper;

namespace ASI.Basecode.Services.Services
{
    public class AddressService : IAddressService
    {
        private readonly IMapper _mapper;
        private readonly IUserProfileRepository _userProfileRepository;

        public AddressService(IMapper mapper, IUserProfileRepository userProfileRepository)
        {
            _mapper = mapper;
            _userProfileRepository = userProfileRepository;
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
        public void RemoveUserAddress(int userAddressID, int userID)
        {
            _userProfileRepository.RemoveUserAddress(userAddressID, userID);
        }
    }
}