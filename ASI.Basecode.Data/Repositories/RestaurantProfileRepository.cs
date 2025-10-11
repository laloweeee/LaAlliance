using System.Linq;
using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;

namespace ASI.Basecode.Data.Repositories
{
    public class RestaurantProfileRepository : BaseRepository, IRestaurantProfile
    {
        public RestaurantProfileRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        /// <summary>
        /// Get the restaurant profile (there should only be one)
        /// </summary>
        /// <returns></returns>
        public Restaurant GetRestaurantProfile()
        {
            return GetDbSet<Restaurant>().FirstOrDefault();
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