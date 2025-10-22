using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.WebApp.Areas.Customer.Models;
using ASI.Basecode.WebApp.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.WebApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class OrderController : ControllerBase<OrderController>
    {
        private readonly IOrderService _orderService;
        private readonly IUserProfileService _userProfileService;

        public OrderController(
            IHttpContextAccessor httpContextAccessor,
            ILoggerFactory loggerFactory,
            IConfiguration configuration,
            IOrderService orderService,
            IUserProfileService userProfileService) : base(httpContextAccessor, loggerFactory, configuration)
        {
            _orderService = orderService;
            _userProfileService = userProfileService;
        }

        public IActionResult Index(string tab = "ongoing")
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            
            var model = new OrderHistoryViewModel
            {
                ActiveTab = tab,
                OngoingOrders = new List<OrderViewModel>(),
                OrderHistory = new List<OrderViewModel>()
            };

            // Get all user orders
            var allOrders = _orderService.GetOrdersByUserId(userId).ToList();

            // Split into ongoing and history
            var ongoingStatuses = new List<OrderStatus> 
            { 
                OrderStatus.Pending, 
                OrderStatus.Processing, 
                OrderStatus.ReadyForPickup, 
                OrderStatus.ReadyForDelivery 
            };

            model.OngoingOrders = allOrders
                .Where(o => ongoingStatuses.Contains(o.OrderStatus))
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            model.OrderHistory = allOrders
                .Where(o => !ongoingStatuses.Contains(o.OrderStatus))
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            return View(model);
        }

        public IActionResult OrderDetails(int orderId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            
            try
            {
                var order = _orderService.GetOrderById(orderId);
                
                // Verify order belongs to current user
                if (order.UserID != userId)
                {
                    TempData["ErrorMessage"] = "Unauthorized access to order.";
                    return RedirectToAction("Index");
                }

                // Instead of trying to render PlaceOrder view, redirect to CheckoutController's PlaceOrder action
                // We'll store the order ID in TempData and let CheckoutController handle loading the order data
                TempData["ViewOrderId"] = orderId;
                return RedirectToAction("ViewOrder", "Checkout", new { area = "Customer", orderId = orderId });
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error retrieving order details for order {OrderId}", orderId);
                TempData["ErrorMessage"] = "Order not found.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reorder(int orderId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            
            try
            {
                var order = _orderService.GetOrderById(orderId);
                
                // Verify order belongs to current user
                if (order.UserID != userId)
                {
                    return Json(new { success = false, message = "Unauthorized access to order." });
                }

                // TODO: Implement reorder logic - add all items from the order to cart
                // This would involve your cart service to add items back to cart
                
                // For now, return success with a message
                return Json(new { 
                    success = true, 
                    message = "Items added to cart successfully!",
                    cartCount = 0 // You would update this with actual cart count
                });
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error reordering order {OrderId}", orderId);
                return Json(new { success = false, message = "Failed to add items to cart." });
            }
        }
    }
}