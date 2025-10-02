using ASI.Basecode.Data.Models;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IServiceData
    {
        ServiceData GetServiceData();
        void EditServiceData(ServiceData data);
    }
}