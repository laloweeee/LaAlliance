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

namespace ASI.Basecode.WebApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CheckoutController : ControllerBase<CheckoutController>
    {
        private readonly IUserProfileService _userProfileService;
        private readonly IAddressService _addressService;
        private readonly IOrderService _orderService;

        public CheckoutController(  IHttpContextAccessor httpContextAccessor,
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
                
                // Create the order request
                var placeOrderRequest = new PlaceOrderRequest
                {
                    UserID = userId,
                    OrderType = model.OrderType,
                    PaymentMethod = model.PaymentMethod,
                    SelectedAddressId = model.SelectedAddressId,
                    DeliveryNotes = model.DeliveryNotes,
                    VoucherCode = model.VoucherCode
                };

                // Place the order
                var orderResult = await _orderService.PlaceOrder(placeOrderRequest);

                // Store order ID in TempData for receipt page
                TempData["OrderID"] = orderResult.OrderID;
                TempData["SuccessMessage"] = "Order placed successfully! Your order number is #" + orderResult.OrderID;
                
                // SET THE CART DATA FOR THE CONFIRMATION PAGE
                model.Cart = currentCart;
                
                // CLEAR THE CART ONLY AFTER WE'VE SAVED THE DATA FOR DISPLAY
                _cartService.ClearCart(userId);
                
                // Return the PlaceOrder view with the complete model
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
    }
}