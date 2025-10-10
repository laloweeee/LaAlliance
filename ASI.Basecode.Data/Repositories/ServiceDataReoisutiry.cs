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

        public DeliveryPolicy GetServiceData()
        {
            return GetDbSet<DeliveryPolicy>().FirstOrDefault();
        }

        public void EditServiceData(DeliveryPolicy data)
        {
            GetDbSet<DeliveryPolicy>().Update(data);
            UnitOfWork.SaveChanges();
        }
    }
}