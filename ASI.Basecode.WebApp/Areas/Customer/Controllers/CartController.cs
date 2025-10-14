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

namespace ASI.Basecode.WebApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    [AutoValidateAntiforgeryToken]
    public class CartController : ControllerBase<CartController>
    {
        private readonly IUserProfileService _userProfileService;

        public CartController(
            IHttpContextAccessor httpContextAccessor,
            ILoggerFactory loggerFactory,
            IConfiguration configuration,
            ICartService cartService,
            IUserProfileService userProfileService
        ) : base(httpContextAccessor, loggerFactory, configuration, null, cartService)
        {
            _userProfileService = userProfileService;
        }

        // GET: /Customer/Cart
        public IActionResult Index()
        {
            var cart = _cartService.GetOrCreateCart(UserId);
            
            // Get customer profile using UserProfileService
            var customerProfile = GetCustomerProfile();
            
            ViewData["CustomerProfile"] = customerProfile;
            return View(cart);
        }

        // POST: /Customer/Cart/UpdateQuantity
        [HttpPost]
        public IActionResult UpdateQuantity(int cartItemID, int quantity)
        {
            if (quantity < 1) quantity = 1;
            
            _cartService.UpdateCartItemQuantity(cartItemID, quantity, UserId);

            return RedirectToAction(nameof(Index));
        }

        // POST: /Customer/Cart/Remove
        [HttpPost]
        public IActionResult Remove(int cartItemID)
        {
            var cart = _cartService.GetOrCreateCart(UserId);
            var itemToRemove = cart.CartItems.FirstOrDefault(i => i.CartItemID == cartItemID);
            var productName = itemToRemove?.ProductName ?? "Item";

            _cartService.RemoveCartItem(cartItemID, UserId);

            TempData["SuccessMessage"] = $"\"{productName}\" has been removed from your cart";
            return RedirectToAction(nameof(Index));
        }

        private AccountViewModel GetCustomerProfile()
        {
            try
            {
                // Get user profile from UserProfileService
                var userProfile = _userProfileService.GetUserProfile(UserId);
                if (userProfile == null) return null;

                // Create AccountViewModel with basic info
                var accountViewModel = new AccountViewModel
                {
                    UserID = userProfile.UserID,
                    Email = userProfile.Email,
                    FirstName = userProfile.FirstName,
                    LastName = userProfile.LastName,
                    ContactNumber = userProfile.ContactNumber,
                    Addresses = new List<UserAddressViewModel>()
                };

                // Try to get user addresses - handle based on actual service model structure
                try
                {
                    var userAddress = _userProfileService.GetUserAddresses(UserId);
                    if (userAddress != null)
                    {
                        // If the service returns a single address, add it to the list
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
                }
                catch
                {
                    // If getting addresses fails, continue with empty address list
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