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

namespace ASI.Basecode.WebApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CheckoutController : ControllerBase<CheckoutController>
    {
        private readonly IUserProfileService _userProfileService;
        private readonly IAddressService _addressService;

        public CheckoutController(  IHttpContextAccessor httpContextAccessor,
                                    ILoggerFactory loggerFactory,
                                    IConfiguration configuration,
                                    IAddressService addressService,
                                    ICartService cartService,
                                    IUserProfileService userProfileService,
                                    IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper, cartService)
        {
            _addressService = addressService;
            _userProfileService = userProfileService;
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
                FullName = $"{userProfile?.FirstName} {userProfile?.LastName}",
                Email = userProfile?.Email,
                ContactNumber = userProfile?.ContactNumber,
                Addresses = _mapper != null
                    ? _mapper.Map<List<UserAddressViewModel>>(addresses)
                    : addresses.Select(a => new UserAddressViewModel()).ToList()
            };

            var googleMapsApiKey = _configuration["GoogleMaps:ApiKey"];
            ViewBag.GoogleMapsApiKey = googleMapsApiKey;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PlaceOrder(CheckoutViewModel model)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
            model.Cart = _cartService.GetOrCreateCart(userId);

            if (!ModelState.IsValid)
            {
                model.AvailableVouchers = new List<string> { "WELCOME10", "FREESHIP", "SAVE20" };
                var userProfile = _userProfileService.GetUserProfile(userId);
                model.FullName = $"{userProfile?.FirstName} {userProfile?.LastName}";
                model.Email = userProfile?.Email;
                model.ContactNumber = userProfile?.ContactNumber;
                model.Addresses = _mapper != null
                    ? _mapper.Map<List<UserAddressViewModel>>(_addressService.GetUserAddresses(userId))
                    : new List<UserAddressViewModel>();
                return View(model);
            }

            // Persist delivery coordinates/address to session so OrderTracking can read them
            try
            {
                var latStr = model.Latitude.HasValue ? model.Latitude.Value.ToString("G", CultureInfo.InvariantCulture) : string.Empty;
                var lngStr = model.Longitude.HasValue ? model.Longitude.Value.ToString("G", CultureInfo.InvariantCulture) : string.Empty;
                _session?.SetString("DeliveryLat", latStr);
                _session?.SetString("DeliveryLng", lngStr);
                _session?.SetString("DeliveryAddress", model.DeliveryAddress ?? string.Empty);
                _session?.SetString("OrderType", model.OrderType ?? "Delivery");
            }
            catch
            {
                // If session is unavailable, continue without failing the order placement
            }

            _cartService.ClearCart(userId);
            TempData["SuccessMessage"] = "Order placed successfully!";

            // Render the confirmation page so users can click "Track your order" and OrderTracking
            // will pick up the coordinates we just saved to session.
            return View("PlaceOrder", model);
        }

        public IActionResult Receipt()
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
            var cart = _cartService.GetOrCreateCart(userId);
            return View("Receipt", cart);
        }
    }
}