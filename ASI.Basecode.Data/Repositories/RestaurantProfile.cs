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

        public Restaurant GetRestaurantProfile()
        {
            return GetDbSet<Restaurant>().FirstOrDefault();
        }

        public void EditRestaurantProfile(Restaurant profile)
        {
            GetDbSet<Restaurant>().Update(profile);
            UnitOfWork.SaveChanges();
        }
    }
}