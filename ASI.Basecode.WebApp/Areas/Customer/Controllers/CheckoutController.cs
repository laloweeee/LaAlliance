using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.WebApp.Areas.Customer.Models;
using AutoMapper; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace ASI.Basecode.WebApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CheckoutController : Controller
    {
        private readonly ICartService _cartService;
        private readonly ILogger<CheckoutController> _logger;
        private readonly IUserProfileService _userProfileService;
        private readonly IAddressService _addressService; 
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public CheckoutController(
            ICartService cartService,
            ILogger<CheckoutController> logger,
            IUserProfileService userProfileService,
            IAddressService addressService, 
            IConfiguration configuration,
            IMapper mapper = null)
        {
            _cartService = cartService;
            _logger = logger;
            _userProfileService = userProfileService;
            _addressService = addressService; 
            _mapper = mapper;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
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
                    : addresses.Select(a => new UserAddressViewModel { /* manual mapping if needed */ }).ToList()
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

            _cartService.ClearCart(userId);
            TempData["SuccessMessage"] = "Order placed successfully!";
            return RedirectToAction("Receipt");
        }

        public IActionResult Receipt()
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
            var cart = _cartService.GetOrCreateCart(userId);
            return View("Receipt", cart);
        }
    }
}