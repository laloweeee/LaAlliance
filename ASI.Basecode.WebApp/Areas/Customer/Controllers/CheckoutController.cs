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
    public class CheckoutController : Controller  // Inherit from ControllerBase if needed for UserId access
    {
        private readonly ICartService _cartService;
        private readonly ILogger<CheckoutController> _logger;
        private readonly IUserProfileService _userProfileService;
        private readonly IMapper _mapper;  // Inject AutoMapper if used
        private readonly IConfiguration _configuration;

        public CheckoutController(
            ICartService cartService,
            ILogger<CheckoutController> logger,
            IUserProfileService userProfileService,
            IConfiguration configuration,
            IMapper mapper = null)  // Optional if AutoMapper is not always required
            
        {
            _cartService = cartService;
            _logger = logger;
            _userProfileService = userProfileService;
            _mapper = mapper;
            _configuration = configuration;
        }

        // GET: /Customer/Checkout
        public IActionResult Index()
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value); 

            // Fetch the cart for the current user
            var cart = _cartService.GetOrCreateCart(userId);
            
            if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty.";
                return RedirectToAction("Index", "Cart");
            }

            // Fetch user profile and addresses
            var userProfile = _userProfileService.GetUserProfile(userId);
            var addresses = _userProfileService.GetUserAddresses(userId);

            var model = new CheckoutViewModel
            {
                Cart = cart,
                AvailableVouchers = new List<string> { "WELCOME10", "FREESHIP", "SAVE20" },
                FullName = $"{userProfile?.FirstName} {userProfile?.LastName}",  // Handle potential null
                Email = userProfile?.Email,
                ContactNumber = userProfile?.ContactNumber,
                Addresses = _mapper != null ? _mapper.Map<List<UserAddressViewModel>>(addresses) : addresses.Select(a => new UserAddressViewModel { /* Manual mapping if needed */ }).ToList()  // Fallback if _mapper is null
            };
            

            var googleMapsApiKey = _configuration["GoogleMaps:ApiKey"];
            ViewBag.GoogleMapsApiKey = "googleMapsApiKey";
            return View(model);
        }

        // POST: /Customer/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(CheckoutViewModel model)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
            model.Cart = _cartService.GetOrCreateCart(userId);  // Re-fetch cart to ensure it's up-to-date

            if (!ModelState.IsValid)
            {
                model.AvailableVouchers = new List<string> { "WELCOME10", "FREESHIP", "SAVE20" };
                // Re-fetch user data if needed for validation errors
                var userProfile = _userProfileService.GetUserProfile(userId);
                model.FullName = $"{userProfile?.FirstName} {userProfile?.LastName}";
                model.Email = userProfile?.Email;
                model.ContactNumber = userProfile?.ContactNumber;
                model.Addresses = _mapper != null ? _mapper.Map<List<UserAddressViewModel>>(_userProfileService.GetUserAddresses(userId)) : new List<UserAddressViewModel>();
                return View(model);
            }

            // Here, save the order to DB, send confirmation, etc. (as per original code)
            TempData["SuccessMessage"] = "Order placed successfully!";
            return RedirectToAction("Receipt");
        }

        // GET: /Customer/Checkout/Receipt
        public IActionResult Receipt()
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
            var cart = _cartService.GetOrCreateCart(userId);  // Fetch for the correct user
            return View("Receipt", cart);
        }
    }
}
