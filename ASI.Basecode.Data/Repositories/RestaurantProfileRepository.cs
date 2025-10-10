using System.Linq;
using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace ASI.Basecode.Data.Repositories
{
    public class RestaurantProfileRepository : BaseRepository, IRestaurantProfileRepository
    {
        public RestaurantProfileRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public RestaurantProfile GetRestaurantProfile()
        {
            return GetDbSet<RestaurantProfile>()
                .Include(p => p.Address)
                .FirstOrDefault();
        }

        public void EditRestaurantProfile(RestaurantProfile profile)
        {
            GetDbSet<RestaurantProfile>().Update(profile);
            UnitOfWork.SaveChanges();
        }
    }
}