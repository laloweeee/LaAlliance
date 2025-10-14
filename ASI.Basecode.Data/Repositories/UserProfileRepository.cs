using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASI.Basecode.Data.Repositories
{
    public class UserProfileRepository : BaseRepository, IUserProfileRepository
    {
        private readonly AsiBasecodeDBContext _dbContext;
        public UserProfileRepository(IUnitOfWork unitOfWork, AsiBasecodeDBContext dbContext) : base(unitOfWork)
        {
            _dbContext = dbContext;
        }

        public IQueryable<UserProfile> GetUserProfile(int userID)
        {
            return _dbContext.UserProfiles.Where(up => up.UserID == userID);
        }

        public void UpdateUserProfile(UserProfile userProfile)
        {
            _dbContext.UserProfiles.Update(userProfile);
            _dbContext.SaveChanges();
        }

        public IQueryable<UserAddress> GetUserAddresses(int userID)
        {
            return _dbContext.UserAddresses.Where(ua => ua.UserID == userID).Include(ua => ua.Address);
        }

        /// <summary>
        /// Adds a new user address, ensuring no duplicate addresses are created.
        /// </summary>
        /// <param name="userAddress"></param>
        public void AddUserAddress(UserAddress userAddress)
        {
            // Check if the Address already exists
            var existingAddress = _dbContext.Addresses
                .FirstOrDefault(a =>
                    a.Street == userAddress.Address.Street &&
                    a.City == userAddress.Address.City &&
                    a.ZipCode == userAddress.Address.ZipCode);

            if (existingAddress == null)
            {
                // Create and save the new address first
                _dbContext.Addresses.Add(userAddress.Address);
                _dbContext.SaveChanges();

                // Assign the new AddressID to the junction entity
                userAddress.AddressID = userAddress.Address.AddressID;
            }
            else
            {
                // If it exists, link to the existing address
                userAddress.AddressID = existingAddress.AddressID;
                userAddress.Address = null;
            }

            // Now add the UserAddress entry
            _dbContext.UserAddresses.Add(userAddress);
            _dbContext.SaveChanges();
        }

        public void UpdateUserAddress(UserAddress userAddress)
        {
            _dbContext.UserAddresses.Update(userAddress);
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Removes the user address.
        /// </summary>
        /// <param name="userAddressID"></param>
        public void RemoveUserAddress(int userAddressID)
        {
            var userAddress = _dbContext.UserAddresses.Find(userAddressID);
            if (userAddress != null)
            {
                _dbContext.UserAddresses.Remove(userAddress);
                _dbContext.SaveChanges();
            }
        }
    }
}