// Libraries
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System;
using System.Threading.Tasks;

// Resources
using static ASI.Basecode.Resources.Constants.Enums;

// Services
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;

// WebApp
using ASI.Basecode.WebApp.Helpers;
using ASI.Basecode.WebApp.Areas.Customer.Models;
using ASI.Basecode.WebApp.Mvc;
using ASI.Basecode.Data.Models;

namespace ASI.Basecode.WebApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CheckoutController : ControllerBase<CheckoutController>
    {
        private readonly IUserProfileService _userProfileService;
        private readonly IAddressService _addressService;
        private readonly IOrderService _orderService;
        private readonly IPromotionService _promotionService;

        public CheckoutController(IHttpContextAccessor httpContextAccessor,
                                    ILoggerFactory loggerFactory,
                                    IConfiguration configuration,
                                    IAddressService addressService,
                                    ICartService cartService,
                                    IUserProfileService userProfileService,
                                    IOrderService orderService,
                                    IPromotionService promotionService,
                                    IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper, cartService)
        {
            _addressService = addressService;
            _userProfileService = userProfileService;
            _orderService = orderService;
            _promotionService = promotionService;
        }

        /// <summary>
        /// Checkout page
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            var googleMapsApiKey = _configuration["GoogleMaps:ApiKey"];
            ViewBag.GoogleMapsApiKey = googleMapsApiKey;

            try
            {
                var userCart = _cartService.GetOrCreateCart(UserId);
                var userAddress = _addressService.GetUserAddresses(UserId);
                var voucherCodes = _promotionService.GetActivePromotions().ToList();

                var model = new CheckoutViewModel
                {
                    Cart = userCart,
                    Addresses = _mapper.Map<List<UserAddressViewModel>>(userAddress),
                    AvailableVouchers = voucherCodes.Select(v => v.PromotionCodes.ToString()).ToList()
                };

                return View(model);
            }
            catch (ArgumentException argEx)
            {
                _logger.LogWarning(argEx, "Invalid argument while retrieving cart for user {UserId}", UserId);
                this.ShowErrorToast(argEx.Message);
                return RedirectToAction("Index", "Cart");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cart for user {UserId}", UserId);
                this.ShowErrorToast("Unable to retrieve cart. Please try again later.");
                return RedirectToAction("Index", "Cart");
            }
        }


        /// <summary>
        /// Place Order
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
        {
            if (model == null)
            {
                this.ShowErrorToast("Invalid order data.");
                return RedirectToAction("Index");
            }

            try
            {

                var userProfile = _userProfileService.GetUserProfile(UserId);
                var cart = _cartService.GetOrCreateCart(UserId);
                var voucherCodes = _promotionService.GetActivePromotions().ToList();
                var userAddress = _mapper.Map<UserAddressViewModel>(_addressService.GetUserAddresses(UserId));

                var orderRequest = new PlaceOrderRequest
                {
                    UserID = UserId,
                    OrderType = model.OrderType,
                    PaymentMethod = model.PaymentMethod,
                    AddressID = OrderType.Delivery == model.OrderType ? model.SelectedAddressId ?? 0 : 0,
                    DeliveryNotes = model.DeliveryNotes,
                    VoucherCode = voucherCodes.Any(v => v.PromotionCodes.ToString() == model.VoucherCode) ? model.VoucherCode : null
                };

                var orderResult = await _orderService.PlaceOrder(orderRequest);
                return RedirectToAction("Receipt", new { orderId = orderResult.OrderID });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error placing order for user {UserId}: {ex.Message}");
                this.ShowErrorToast(ex.Message);
                return View("Index");
            }
        }

        /// <summary>
        /// Order Receipt
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public IActionResult Receipt(int? orderId)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);

            try
            {
                // Get the order ID from parameter or TempData
                int orderIdToView = orderId ?? (TempData["OrderID"] != null ? (int)TempData["OrderID"] : 0);

                if (orderIdToView == 0)
                {
                    this.ShowErrorToast("Order not found.");
                    return RedirectToAction("Index", "Home");
                }

                // Get the order details
                var order = _orderService.GetOrderById(orderIdToView);

                // Verify that the order belongs to the current user
                if (order.UserID != userId)
                {
                    this.ShowErrorToast("Unauthorized access to order.");
                    return RedirectToAction("Index", "Home");
                }

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving order receipt for user {UserId}", userId);
                this.ShowErrorToast("Unable to retrieve order details." + ex.Message);
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CancelOrder([FromBody] CancelOrderRequest request)
        {
            try
            {
                // Parse orderId to int
                if (!int.TryParse(request.OrderId, out int orderIdInt))
                {
                    return Json(new { success = false, message = "Invalid order ID." });
                }

                // Get current user ID
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Json(new { success = false, message = "User not authenticated." });
                }

                // Get the order from database to check status
                var order = _orderService.GetOrderById(orderIdInt);

                if (order == null)
                {
                    return Json(new { success = false, message = "Order not found." });
                }

                // Verify order belongs to current user
                if (order.UserID != userId)
                {
                    return Json(new { success = false, message = "Unauthorized access to order." });
                }

                // Check if order can be cancelled (only Pending orders)
                if (order.OrderStatus != OrderStatus.Pending)
                {
                    return Json(new { success = false, message = "Order can no longer be cancelled. It is already being prepared." });
                }

                // Cancel the order using the service method
                _orderService.CancelOrder(orderIdInt, userId);

                return Json(new {
                    success = true,
                    message = "Order cancelled successfully!",
                    orderId = request.OrderId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling order {OrderId}", request.OrderId);
                return Json(new {
                    success = false,
                    message = "An error occurred while cancelling the order. Please try again."
                });
            }
        }

        [HttpGet]
        [ValidateAntiForgeryToken]
        public IActionResult GetOrderStatus(string orderId)
        {
            try
            {
                // Parse orderId to int
                if (!int.TryParse(orderId, out int orderIdInt))
                {
                    return Json(new { status = "Unknown" });
                }

                // Get current user ID
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Json(new { status = "Unknown" });
                }

                var order = _orderService.GetOrderById(orderIdInt);

                // Verify order belongs to current user
                if (order == null || order.UserID != userId)
                {
                    return Json(new { status = "Unknown" });
                }

                return Json(new { status = order.OrderStatus.ToString() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting order status {OrderId}", orderId);
                return Json(new { status = "Unknown" });
            }
        }

        // Request model
        public class CancelOrderRequest
        {
            public string OrderId { get; set; }
        }
    }

}