using ASI.Basecode.Data.Models;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using ASI.Basecode.WebApp.Areas.Restaurant.Models; // for RestaurantProfileViewModel

namespace ASI.Basecode.WebApp.Areas.Restaurant.Controller
{
    [Authorize(Policy = "Restaurant")]
    [Area("Restaurant")]
    public class RestaurantController : ControllerBase<RestaurantController>
    {
        public RestaurantController(
            IHttpContextAccessor httpContextAccessor,
            ILoggerFactory loggerFactory,
            IConfiguration configuration,
            IMapper mapper = null
        ) : base(httpContextAccessor, loggerFactory, configuration, mapper) { }

        // =======================
        // Landing / Redirect
        // =======================
        public IActionResult Index()
        {
            // Keep user info available if needed
            ViewBag.UserEmail = User.Identity?.Name;
            ViewBag.UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Always redirect to Dashboard after login
            return RedirectToAction(nameof(Dashboard));
        }

        // =======================
        // Dashboard
        // =======================
        [HttpGet]
        public IActionResult Dashboard()
        {
            return View(); // Views/Restaurant/Dashboard.cshtml
        }

        // =======================
        // Restaurant Profile
        // =======================
        [HttpGet]
        public IActionResult RestaurantProfile()
        {
            // Dummy data for now (replace with DB call later)
            var vm = new RestaurantProfileViewModel
            {
                RestaurantName = "FastFood Restaurant",
                Address = "123 Main Street, City, State 12345",
                Phone = "(555) 123-4567",
                Email = "info@fastfood.com",
                DeliveryRadius = 5,
                DeliveryFee = 3.99m,
                MinimumOrderAmount = 15.00m,
                OpenTime = "09:00",
                CloseTime = "21:00",
                OpenAllDay = false
            };

            return View(vm); // Views/Restaurant/RestaurantProfile.cshtml
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult UpdateProfile(RestaurantProfileViewModel vm)
        {
            // Only validate profile fields
            ModelState.Remove(nameof(vm.DeliveryRadius));
            ModelState.Remove(nameof(vm.DeliveryFee));
            ModelState.Remove(nameof(vm.MinimumOrderAmount));
            ModelState.Remove(nameof(vm.OpenTime));
            ModelState.Remove(nameof(vm.CloseTime));
            ModelState.Remove(nameof(vm.OpenAllDay));

            if (!ModelState.IsValid)
                return View("RestaurantProfile", vm);

            // TODO: Save to DB
            TempData["Toast"] = "Restaurant profile updated.";
            return RedirectToAction(nameof(RestaurantProfile));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult UpdateOperations(RestaurantProfileViewModel vm)
        {
            // Only validate delivery/ops fields
            ModelState.Remove(nameof(vm.RestaurantName));
            ModelState.Remove(nameof(vm.Address));
            ModelState.Remove(nameof(vm.Phone));
            ModelState.Remove(nameof(vm.Email));

            if (!ModelState.IsValid)
                return View("RestaurantProfile", vm);

            // TODO: Save to DB
            TempData["Toast"] = "Delivery & operations updated.";
            return RedirectToAction(nameof(RestaurantProfile));
        }

        // =======================
        // Menu
        // =======================
        [HttpGet]
        public IActionResult Menu()
        {
            return View(); // Views/Restaurant/Menu.cshtml
        }

        // =======================
        // Promotions
        // =======================
        [HttpGet]
        public IActionResult Promotions()
        {
            return View(); // Views/Restaurant/Promotions.cshtml
        }

        // =======================
        // Reports
        // =======================
        [HttpGet]
        public IActionResult Reports()
        {
            return View(); // Views/Restaurant/Reports.cshtml
        }

        // =======================
        // Staff
        // =======================
        [HttpGet]
        public IActionResult Staff()
        {
            return View(); // Views/Restaurant/Staff.cshtml
        }
    }
}
