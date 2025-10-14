using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using ASI.Basecode.Services.Hubs;
using ASI.Basecode.Services.Interfaces;

namespace ASI.Basecode.Services.Services
{
    public class OrderNotificationService : IOrderNotificationService
    {
        private readonly IHubContext<OrderHub> _hubContext;

        public OrderNotificationService(IHubContext<OrderHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyNewOrder(string orderID, string orderDetails)
        {
            await _hubContext.Clients.Group("Restaurant").SendAsync("ReceiveNewOrder", new
            {
                orderID = orderID,
                orderDetails = orderDetails,
                timeStamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                message = "A new order has been placed."
            });
        }

        public async Task NotifyOrderStatusChange(string orderID, string status, string message)
        {
            await _hubContext.Clients.Group($"Order-{orderID}").SendAsync("ReceiveOrderStatusChange", new
            {
                orderID = orderID,
                status = status,
                timeStamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                message = message
            });
        }

        public async Task NotifyOrderCancellation(string orderID, string reason)
        {
            await _hubContext.Clients.Group($"Order-{orderID}").SendAsync("ReceiveOrderCancellation", new
            {
                orderID = orderID,
                reason = reason,
                timeStamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                message = "Your order has been cancelled."
            });
        }
    }
}