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

namespace ASI.Basecode.WebApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    [AutoValidateAntiforgeryToken]
    public class CartController : ControllerBase<CartController>
    {
        private readonly IUserProfileService _userProfileService;
        private readonly IAddressService _addressService;

        public CartController(
                                IHttpContextAccessor httpContextAccessor,
                                ILoggerFactory loggerFactory,
                                IConfiguration configuration,
                                ICartService cartService,
                                IUserProfileService userProfileService,
                                IAddressService addressService
                            ) : base(httpContextAccessor, loggerFactory, configuration, null, cartService)
        {
            _userProfileService = userProfileService;
            _addressService = addressService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.UserName = User.FindFirst(ClaimTypes.Name)?.Value;

            // Get or create cart for the user
            var cart = _cartService.GetOrCreateCart(UserId);
            return View();
        }

        /// <summary>
        /// Updates the quantity of a cart item
        /// </summary>
        /// <param name="cartItemID"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        [HttpPost]
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
    
        public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
        {
            if (!ModelState.IsValid)
            {
                this.ShowErrorToast("Please correct the errors in the form.");
                return View("CheckOut", model);
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

                this.ShowSuccessToast("Your order has been placed successfully!");
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                this.ShowErrorToast("Failed to place order: " + ex.Message);
                return View("CheckOut", model);
            }
        }
    }
}