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

        public void AddUserAddress(UserAddress userAddress)
        {
            _dbContext.UserAddresses.Add(userAddress);
            _dbContext.SaveChanges();
        }

        public void UpdateUserAddress(UserAddress userAddress)
        {
            _dbContext.UserAddresses.Update(userAddress);
            _dbContext.SaveChanges();
        }

        public void RemoveUserAddress(int userAddressID)
        {
            var userAddress = _dbContext.UserAddresses.Find(userAddressID);
            if (userAddress != null)
            {
                _dbContext.UserAddresses.Remove(userAddress);
            }
        }
    }
}