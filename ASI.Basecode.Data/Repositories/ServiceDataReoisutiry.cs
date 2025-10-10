using System.Linq;
using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;

namespace ASI.Basecode.Data.Repositories
{
    public class ServiceDataRepository : BaseRepository, IServiceData
    {
        public ServiceDataRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public ServiceData GetServiceData()
        {
            return GetDbSet<ServiceData>().FirstOrDefault();
        }

        public void EditServiceData(ServiceData data)
        {
            GetDbSet<ServiceData>().Update(data);
            UnitOfWork.SaveChanges();
        }
    }
}