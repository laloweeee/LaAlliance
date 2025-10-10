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

        public RestaurantProfile GetRestaurantProfile()
        {
            return GetDbSet<RestaurantProfile>().FirstOrDefault();
        }

        public void EditRestaurantProfile(RestaurantProfile profile)
        {
            GetDbSet<RestaurantProfile>().Update(profile);
            UnitOfWork.SaveChanges();
        }
    }
}