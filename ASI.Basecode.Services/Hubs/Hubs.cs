using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System;

namespace ASI.Basecode.Services.Hubs
{
    public class OrderHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var userID = Context.UserIdentifier;
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinRestaurantGroup()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Restaurant");
        }

        public async Task JoinOrderGroup(string orderID)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Order-{orderID}");
        }

        public async Task JoinUserOrdersGroup(string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"UserOrders-{userId}");
            await Clients.Caller.SendAsync("JoinedGroup", $"Successfully joined user orders group: {userId}");
        }

        public async Task LeaveOrderGroup(string orderID)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Order-{orderID}");
        }
    }
}