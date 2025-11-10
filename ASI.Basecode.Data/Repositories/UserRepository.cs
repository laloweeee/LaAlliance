using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Repositories
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        private readonly AsiBasecodeDBContext _dbContext;
        public UserRepository(IUnitOfWork unitOfWork, AsiBasecodeDBContext dbContext) : base(unitOfWork)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns></returns>
        public IQueryable<User> GetUsers()
        {
            return _dbContext.Users
                .Include(u => u.UserProfile)
                .Include(u => u.RestaurantStaff);
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public User GetUserByID(int userID)
        {
            return _dbContext.Users
                .Include(u => u.UserProfile)
                .Include(u => u.RestaurantStaff)
                .FirstOrDefault(u => u.UserID == userID);
        }

        /// <summary>
        /// Check if a user exists by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool UserExists(string email)
        {
            return _dbContext.Users.Any(x => x.Email == email);
        }

        /// <summary>
        /// Add a new user
        /// </summary>
        /// <param name="user"></param>
        public void AddUser(User user)
        {
            _dbContext.Users.Add(user);
            UnitOfWork.SaveChanges();
        }

        /// <summary>
        /// Update an existing user
        /// </summary>
        /// <param name="user"></param>
        public void UpdateUser(User user)
        {
            _dbContext.Users.Update(user);
            UnitOfWork.SaveChanges();
        }

        /// <summary>
        /// Delete a user and all related data
        /// </summary>
        /// <param name="userID"></param>
        public void DeleteUser(int userID)
        {
            var user = _dbContext.Users
                .Include(u => u.UserProfile)
                .Include(u => u.RestaurantStaff)
                .FirstOrDefault(u => u.UserID == userID);

            if (user != null)
            {
                // Manually cascade delete in the correct order
                
                // 1. Delete RestaurantStaff record first (has FK to User)
                if (user.RestaurantStaff != null)
                {
                    _dbContext.RestaurantStaff.Remove(user.RestaurantStaff);
                }
                
                // 2. Delete UserProfile if exists (has FK to User)
                if (user.UserProfile != null)
                {
                    _dbContext.UserProfiles.Remove(user.UserProfile);
                }
                
                // 3. Delete User record last
                _dbContext.Users.Remove(user);
                
                _dbContext.SaveChanges();
            }
            else
            {
                throw new ArgumentException("User not found.");
            }
        }
    }
}
