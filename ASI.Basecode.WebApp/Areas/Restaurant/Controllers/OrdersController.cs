using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System;
using ASI.Basecode.Resources.Constants;
using System.Linq;
using ASI.Basecode.Services.Interfaces;
using System.Threading.Tasks;

namespace ASI.Basecode.WebApp.Areas.Restaurant.Controllers
{
    [Authorize(Policy = "Restaurant")]
    [Area("Restaurant")]
    public class OrdersController : ControllerBase<OrdersController>
    {
        private readonly IOrderService _orderService;
        
        public OrdersController(
            IHttpContextAccessor httpContextAccessor,
            ILoggerFactory loggerFactory,
            IConfiguration configuration,
            IMapper mapper,
            IOrderService orderService
        ) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                ViewBag.UserEmail = User.Identity?.Name;
                ViewBag.UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var orders = _orderService.GetAllOrders();

                _logger.LogInformation("Orders loaded successfully. Number of orders: {OrderCount}", orders.Count());

                return View(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading Orders index page.");
                return View();
            }
        }

        public IActionResult ViewOrder(int id)
        {
            try
            {
                ViewBag.UserEmail = User.Identity?.Name;
                ViewBag.UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var order = _orderService.GetOrderById(id);

                _logger.LogInformation("Order details loaded successfully for OrderID: {OrderID}", id);

                return View(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading Orders view page.");
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Accept a pending order
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptOrder(int orderId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                await _orderService.UpdateOrderStatus(orderId, userId, Enums.OrderStatus.Processing);

                TempData["SuccessMessage"] = "Order accepted successfully!";
                return Json(new { success = true, message = "Order accepted successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error accepting order {OrderId}", orderId);
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Update order status
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, string status)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var orderStatus = Enum.Parse<Enums.OrderStatus>(status);

                await _orderService.UpdateOrderStatus(orderId, userId, orderStatus);

                TempData["SuccessMessage"] = $"Order status updated to {status}!";
                return Json(new { success = true, message = $"Order status updated to {status}!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order {OrderId} status", orderId);
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
