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

            // Get customer profile using UserProfileService
            var customerProfile = GetCustomerProfile();

            ViewData["CustomerProfile"] = customerProfile;
            return View(cart);
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

        /// <summary>
        /// Gets the customer profile including addresses
        /// </summary>
        /// <returns></returns>
        private AccountViewModel GetCustomerProfile()
        {
            try
            {
                var userProfile = _userProfileService.GetUserProfile(UserId);
                if (userProfile == null) return null;

                var accountViewModel = new AccountViewModel
                {
                    UserID = userProfile.UserID,
                    Email = userProfile.Email,
                    FirstName = userProfile.FirstName,
                    LastName = userProfile.LastName,
                    ContactNumber = userProfile.ContactNumber,
                    Addresses = new List<UserAddressViewModel>()
                };

                // Get User Addresses
                var userAddresses = _addressService.GetUserAddresses(UserId);

                foreach (var userAddress in userAddresses)
                {
                    accountViewModel.Addresses.Add(new UserAddressViewModel
                    {
                        UserAddressID = userAddress.UserAddressID,
                        AddressID = userAddress.AddressID,
                        IsDefault = userAddress.IsDefault,
                        AddressType = userAddress.AddressType,
                        AddressNote = userAddress.AddressNote,
                        Street = userAddress.Street,
                        Barangay = userAddress.Barangay,
                        City = userAddress.City,
                        Province = userAddress.Province,
                        ZipCode = userAddress.ZipCode,
                        Longitude = userAddress.Longitude,
                        Latitude = userAddress.Latitude
                    });
                }

                return accountViewModel;
            }
            catch
            {
                // Return null if there's an error getting the profile
                return null;
            }
        }
    }
}