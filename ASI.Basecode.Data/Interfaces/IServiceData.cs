using ASI.Basecode.Data.Models;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IServiceData
    {
        DeliveryPolicy GetServiceData();
        void EditServiceData(DeliveryPolicy data);
    }
}