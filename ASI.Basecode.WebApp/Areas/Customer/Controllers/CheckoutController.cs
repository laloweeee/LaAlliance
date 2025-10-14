using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.WebApp.Areas.Customer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace ASI.Basecode.WebApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CheckoutController : Controller
    {
        private readonly ICartService _cartService;
        private readonly ILogger<CheckoutController> _logger;

        public CheckoutController(ICartService cartService, ILogger<CheckoutController> logger)
        {
            _cartService = cartService;
            _logger = logger;
        }

        // GET: /Customer/Checkout
        public IActionResult Index()
        {
            // For demo, use userId = 1
            int userId = 1;
            var cart = _cartService.GetOrCreateCart(userId);

            if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty.";
                return RedirectToAction("Index", "Cart");
            }

            var model = new CheckoutViewModel
            {
                Cart = cart,
                AvailableVouchers = new System.Collections.Generic.List<string> { "WELCOME10", "FREESHIP", "SAVE20" }
            };

            return View(model);
        }

        // POST: /Customer/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(CheckoutViewModel model)
        {
            // For demo, use userId = 1
            int userId = 1;
            model.Cart = _cartService.GetOrCreateCart(userId);

            if (!ModelState.IsValid)
            {
                model.AvailableVouchers = new System.Collections.Generic.List<string> { "WELCOME10", "FREESHIP", "SAVE20" };
                return View(model);
            }

            // Here, save the order to DB, send confirmation, etc.
            TempData["SuccessMessage"] = "Order placed successfully!";
            return RedirectToAction("Receipt");
        }

        // GET: /Customer/Checkout/Receipt
        public IActionResult Receipt()
        {
            // For demo, use userId = 1
            int userId = 1;
            var cart = _cartService.GetOrCreateCart(userId);

            return View("Receipt", cart);
        }
    }
}