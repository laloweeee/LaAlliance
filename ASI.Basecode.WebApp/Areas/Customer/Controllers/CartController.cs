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
using System;
using ASI.Basecode.WebApp.Helpers;
using System.Threading.Tasks;
using AutoMapper;
using ASI.Basecode.Data.Models;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.WebApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : ControllerBase<CartController>
    {
        private readonly IUserProfileService _userProfileService;
        private readonly IAddressService _addressService;
        private readonly IOrderService _orderService;
        private readonly IPromotionService _promotionService;

        public CartController(
                                IHttpContextAccessor httpContextAccessor,
                                ILoggerFactory loggerFactory,
                                IConfiguration configuration,
                                ICartService cartService,
                                IUserProfileService userProfileService,
                                IAddressService addressService,
                                IOrderService orderService,
                                IPromotionService promotionService,
                                IMapper mapper
                            ) : base(httpContextAccessor, loggerFactory, configuration, mapper, cartService)
        {
            _userProfileService = userProfileService;
            _addressService = addressService;
            _orderService = orderService;
            _promotionService = promotionService;
        }

        /// <summary>
        /// Displays the cart page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.UserName = User.FindFirst(ClaimTypes.Name)?.Value;

            // Get or create cart for the user
            var cart = _cartService.GetOrCreateCart(UserId);
            return View(cart);
        }

        /// <summary>
        /// Updates the quantity of a cart item
        /// </summary>
        /// <param name="cartItemID"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult ApplyVoucher(string code)
        {
            try
            {
                if (UserId == 0) return Json(new { success = false, message = "Please log in to apply voucher." });

                var cart = _cartService.GetOrCreateCart(UserId);
                var validation = _promotionService.ValidatePromotionCode(code, cart);

                if (!validation.IsValid)
                {
                    return Json(new { success = false, message = validation.Message });
                }

                var newTotal = Math.Max(0, cart.Total - validation.DiscountAmount);

                return Json(new
                {
                    success = true,
                    discountAmount = validation.DiscountAmount,
                    newTotal = newTotal,
                    message = validation.Message
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Failed to apply voucher: " + ex.Message });
            }
        }

        [ValidateAntiForgeryToken]
        public IActionResult UpdateQuantity(int cartItemID, int quantity)
        {
            try
            {
                if (quantity < 1) quantity = 1;

                _cartService.UpdateCartItemQuantity(cartItemID, quantity, UserId);

                // Get updated cart data
                var cart = _cartService.GetOrCreateCart(UserId);

                // Find the specific item that was updated
                var updatedItem = cart.CartItems.FirstOrDefault(x => x.CartItemID == cartItemID);

                if (updatedItem == null)
                {
                    return Json(new { success = false, message = "Cart item not found" });
                }

                // Calculate totals
                var totalItems = cart.CartItems.Sum(x => x.Quantity);
                var subTotal = cart.CartItems.Sum(x => x.TotalPrice);
                var deliveryFee = cart.DeliveryFee;
                var total = subTotal + deliveryFee;

                return Json(new
                {
                    success = true,
                    newQuantity = updatedItem.Quantity,
                    itemTotalPrice = updatedItem.TotalPrice,
                    totalItems = totalItems,
                    subTotal = subTotal,
                    deliveryFee = deliveryFee,
                    total = total
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Failed to update quantity: " + ex.Message });
            }
        }

        /// <summary>
        /// Removes an item from the cart
        /// </summary>
        /// <param name="cartItemID"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Remove(int id)
        {
            try
            {
                var cart = _cartService.GetOrCreateCart(UserId);
                var itemToRemove = cart.CartItems.FirstOrDefault(i => i.CartItemID == id);
                var productName = itemToRemove?.ProductName ?? "Item";

                _cartService.RemoveCartItem(id, UserId);

                this.ShowSuccessToast($"\"{productName}\" has been removed from your cart");
            }
            catch (Exception ex)
            {
                this.ShowErrorToast("Failed to remove item: " + ex.Message);
            }

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Displays the checkout page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> CheckOut()
        {
            var googleApiKey = _configuration["GoogleMaps:ApiKey"];
            ViewBag.GoogleMapsApiKey = googleApiKey;

            if (UserId == 0)
            {
                _logger.LogError("USER ID IS NULL or ZERO");
                this.ShowErrorToast("Please log in to continue checkout.");
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var cart = _cartService.GetOrCreateCart(UserId);

                // Check if cart is empty
                if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
                {
                    this.ShowWarningToast("Your cart is empty.");
                    return RedirectToAction("Index", "Cart");
                }

                // Get user addresses with null check
                var addresses = _addressService.GetUserAddresses(UserId) ?? new List<UserAddressServiceModel>();
                var userAddresses = _mapper.Map<List<UserAddressViewModel>>(addresses);

                //var vouchers = _promotionService.GetActivePromotions()?.ToList() ?? new List<RestaurantPromotions>();

                var model = new CheckoutViewModel
                {
                    Cart = cart,
                    Addresses = userAddresses,
                    // Initialize other properties as needed
                    OrderType = OrderType.Delivery // Set default
                };

                return View(model);
            }
            catch (Exception ex)
            {
                this.ShowErrorToast("Failed to load checkout page.");
                _logger.LogError(ex, "Error in CheckOut: {Message}", ex.Message);
                return RedirectToAction("Index", "Cart");
            }
        }

        /// <summary>
        /// Places an order based on the checkout model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
        {
            if (!ModelState.IsValid)
            {
                this.ShowErrorToast("Please correct the errors in the form.");
                return RedirectToAction("CheckOut");
            }

            try
            {
                var orderRequest = new PlaceOrderRequest
                {
                    UserID = UserId,
                    SelectedAddressId = model.SelectedAddressId,
                    OrderType = model.OrderType,
                    PaymentMethod = model.PaymentMethod,
                    DeliveryNotes = model.DeliveryNotes,
                    VoucherCode = model.VoucherCode
                };

                _logger.LogInformation($"Order Placed with payment method: {orderRequest.PaymentMethod}");

                var order = await _orderService.PlaceOrder(orderRequest);

                this.ShowSuccessToast("Your order has been placed successfully!");
                return RedirectToAction("Index", "Order", new { area = "Customer", orderId = order.OrderID });
            }
            catch (Exception ex)
            {
                this.ShowErrorToast("Failed to place order: " + ex.Message);
                return RedirectToAction("CheckOut");
            }
        }
    }
}