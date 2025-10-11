using System.Linq;
using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ASI.Basecode.Data.Repositories
{
    public class RestaurantProfileRepository : BaseRepository, IRestaurantProfileRepository
    {

        private readonly AsiBasecodeDBContext _dbContext;
        public RestaurantProfileRepository(IUnitOfWork unitOfWork, AsiBasecodeDBContext dbContext) : base(unitOfWork)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Get the restaurant profile (there should only be one)
        /// </summary>
        /// <returns></returns>
        public Restaurant GetRestaurantProfile()
        {
            return _dbContext.Restaurants
                        .Include(r => r.RestaurantAddress)
                            .ThenInclude(ra => ra.Address)
                        .FirstOrDefault();
        }

        /// <summary>
        /// Edit the restaurant profile
        /// </summary>
        /// <param name="profile"></param>
        public void EditRestaurantProfile(Restaurant profile)
        {
            GetDbSet<Restaurant>().Update(profile);
            UnitOfWork.SaveChanges();
        }
    }
}