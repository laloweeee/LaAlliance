using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using ASI.Basecode.WebApp.Areas.Customer.Models;
using System.Security.Claims;
using ASI.Basecode.Services.ServiceModels;
using System;
using System.Threading.Tasks;
using ASI.Basecode.WebApp.Helpers;
using static ASI.Basecode.Resources.Constants.Enums;
using ASI.Basecode.Resources.Constants;

namespace ASI.Basecode.WebApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CheckoutController : ControllerBase<CheckoutController>
    {
        private readonly IUserProfileService _userProfileService;
        private readonly IAddressService _addressService;
        private readonly IOrderService _orderService;

        public CheckoutController(IHttpContextAccessor httpContextAccessor,
                                    ILoggerFactory loggerFactory,
                                    IConfiguration configuration,
                                    IAddressService addressService,
                                    ICartService cartService,
                                    IUserProfileService userProfileService,
                                    IOrderService orderService,
                                    IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper, cartService)
        {
            _addressService = addressService;
            _userProfileService = userProfileService;
            _orderService = orderService;
        }

        public IActionResult Index()
        {
            ViewBag.UserName = User.FindFirst(ClaimTypes.Name)?.Value;
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);

            var cart = _cartService.GetOrCreateCart(userId);

            if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty.";
                return RedirectToAction("Index", "Cart");
            }

            var userProfile = _userProfileService.GetUserProfile(userId);
            var addresses = _addressService.GetUserAddresses(userId);

            var model = new CheckoutViewModel
            {
                Cart = cart,
                AvailableVouchers = new List<string> { "WELCOME10", "FREESHIP", "SAVE20" },
                Addresses = _mapper != null
                    ? _mapper.Map<List<ASI.Basecode.WebApp.Areas.Customer.Models.UserAddressViewModel>>(addresses)
                    : addresses.Select(a => new ASI.Basecode.WebApp.Areas.Customer.Models.UserAddressViewModel()).ToList(),
                // ADD USER INFORMATION
                FullName = $"{userProfile?.FirstName} {userProfile?.LastName}",
                Email = userProfile?.Email,
                ContactNumber = userProfile?.ContactNumber
            };

            var googleMapsApiKey = _configuration["GoogleMaps:ApiKey"];
            ViewBag.GoogleMapsApiKey = googleMapsApiKey;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);

            try
            {
                        // Validate the model
                if (!ModelState.IsValid)
                {
                    // Reload necessary data for the view
                    model.Cart = _cartService.GetOrCreateCart(userId);
                    model.AvailableVouchers = new List<string> { "WELCOME10", "FREESHIP", "SAVE20" };
                    model.Addresses = _mapper != null
                        ? _mapper.Map<List<ASI.Basecode.WebApp.Areas.Customer.Models.UserAddressViewModel>>(_addressService.GetUserAddresses(userId))
                        : new List<ASI.Basecode.WebApp.Areas.Customer.Models.UserAddressViewModel>();

                    this.ShowErrorToast("Please fill in all required fields.");
                    return View("Index", model);
                }

                // Validate delivery address for delivery orders
                if (model.OrderType == "Delivery" && !model.SelectedAddressId.HasValue)
                {
                    model.Cart = _cartService.GetOrCreateCart(userId);
                    model.AvailableVouchers = new List<string> { "WELCOME10", "FREESHIP", "SAVE20" };
                    model.Addresses = _mapper != null
                        ? _mapper.Map<List<ASI.Basecode.WebApp.Areas.Customer.Models.UserAddressViewModel>>(_addressService.GetUserAddresses(userId))
                        : new List<ASI.Basecode.WebApp.Areas.Customer.Models.UserAddressViewModel>();

                    this.ShowErrorToast("Please select a delivery address.");
                    return View("Index", model);
                }

                // GET THE CART DATA BEFORE CLEARING IT
                var currentCart = _cartService.GetOrCreateCart(userId);

                // CREATE THE ORDER REQUEST AND PLACE ORDER
                var placeOrderRequest = new PlaceOrderRequest
                {
                    UserID = userId,
                    OrderType = model.OrderType,
                    PaymentMethod = model.PaymentMethod,
                    SelectedAddressId = model.SelectedAddressId,
                    DeliveryNotes = model.DeliveryNotes,
                    VoucherCode = model.VoucherCode
                };

                var orderResult = await _orderService.PlaceOrder(placeOrderRequest);

                // STORE ORDER ID IN TEMPDATA FOR RECEIPT PAGE
                TempData["OrderID"] = orderResult.OrderID;
                TempData["SuccessMessage"] = "Order placed successfully! Your order number is #" + orderResult.OrderID;

                // LOAD USER PROFILE AND ADDRESSES FOR THE CONFIRMATION PAGE
                var userProfile = _userProfileService.GetUserProfile(userId);
                var addresses = _addressService.GetUserAddresses(userId);

                // UPDATE THE MODEL WITH ALL NECESSARY DATA
                model.Cart = currentCart;
                model.Addresses = _mapper != null
                    ? _mapper.Map<List<UserAddressViewModel>>(addresses)
                    : addresses.Select(a => new UserAddressViewModel()).ToList();
                model.FullName = $"{userProfile?.FirstName} {userProfile?.LastName}";
                model.Email = userProfile?.Email;
                model.ContactNumber = userProfile?.ContactNumber;

                // CLEAR THE CART ONLY AFTER WE'VE SAVED THE DATA FOR DISPLAY
                _cartService.ClearCart(userId);

                // RETURN THE PLACEORDER VIEW WITH THE COMPLETE MODEL
                return View("PlaceOrder", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error placing order for user {UserId}", userId);

                // Reload necessary data for the view
                model.Cart = _cartService.GetOrCreateCart(userId);
                model.AvailableVouchers = new List<string> { "WELCOME10", "FREESHIP", "SAVE20" };
                model.Addresses = _mapper != null
                    ? _mapper.Map<List<UserAddressViewModel>>(_addressService.GetUserAddresses(userId))
                    : new List<UserAddressViewModel>();

                TempData["ErrorMessage"] = ex.Message;
                return View("Index", model);
            }
        }
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
        [Area("Customer")]
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
        [Area("Customer")]
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