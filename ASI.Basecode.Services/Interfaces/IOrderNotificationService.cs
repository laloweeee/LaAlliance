using System.Threading.Tasks;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IOrderNotificationService
    {
        Task NotifyNewOrder(string orderId, string orderDetails);
        Task NotifyOrderStatusChange(string orderId, string status, string message);
        Task NotifyOrderCancellation(string orderId, string reason);
    }
}