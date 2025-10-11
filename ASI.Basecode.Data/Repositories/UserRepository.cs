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
        public IQueryable<User> GetUserByID(int userID)
        {
            return _dbContext.Users
                .Include(u => u.UserProfile)
                .Include(u => u.RestaurantStaff)
                .Where(x => x.UserID == userID);
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
    }
}
