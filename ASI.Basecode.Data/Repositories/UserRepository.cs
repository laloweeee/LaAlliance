using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Repositories
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(IUnitOfWork unitOfWork) : base(unitOfWork) 
        {

        }

        public IQueryable<User> GetUsers()
        {
            return GetDbSet<User>();
        }

        public IQueryable<User> GetUserById(int userId)
        {
            return GetDbSet<User>().Where(x => x.UserId == userId);
        }

        public bool UserExists(string email)
        {
            return GetDbSet<User>().Any(x => x.Email == email);
        }

        public void AddUser(User user)
        {
            user.CreatedTime = DateTime.UtcNow;
            user.UpdatedTime = DateTime.UtcNow;
            GetDbSet<User>().Add(user);
            UnitOfWork.SaveChanges();
        }
    }
}
