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

        public IQueryable<User> GetUserById(int userID)
        {
            return GetDbSet<User>().Where(x => x.UserID == userID);
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

        public void UpdateUser(User user)
        {
            user.UpdatedTime = DateTime.UtcNow;
            GetDbSet<User>().Update(user);
            UnitOfWork.SaveChanges();
        }
    }
}
