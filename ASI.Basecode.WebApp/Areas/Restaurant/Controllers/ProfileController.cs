using ASI.Basecode.Data.Models;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using ASI.Basecode.WebApp.Areas.Restaurant.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using System;

namespace ASI.Basecode.WebApp.Areas.Restaurant.Controllers
{
    [Authorize(Policy = "Restaurant")]
    [Area("Restaurant")]
    public class ProfileController : ControllerBase<ProfileController>
    {
        private readonly IRestaurantProfileService _restaurantProfileService;

        public ProfileController(
                                IHttpContextAccessor httpContextAccessor,
                                ILoggerFactory loggerFactory,
                                IConfiguration configuration,
                                IMapper mapper,
                                IRestaurantProfileService restaurantProfileService
                            ) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _restaurantProfileService = restaurantProfileService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var profile = _restaurantProfileService.GetRestaurantProfile();
            var model = _mapper.Map<RestaurantProfileViewModel>(profile);
            var googleApiKey = _configuration["GoogleMaps:ApiKey"];

            ViewBag.GoogleMapsApiKey = googleApiKey;
            return View(model);
        }

        [HttpGet]
        public IActionResult RestaurantProfileForm()
        {
            var googleApiKey = _configuration["GoogleMaps:ApiKey"];

            ViewBag.GoogleMapsApiKey = googleApiKey;
            var model = new RestaurantViewModel
            {
                Profile = _mapper.Map<RestaurantProfileViewModel>(_restaurantProfileService.GetRestaurantProfile())
            };

            return View(model);
        }

        public IActionResult UpdateRestaurantProfile(RestaurantViewModel model)
        {
            var googleApiKey = _configuration["GoogleMaps:ApiKey"];
            ViewBag.GoogleMapsApiKey = googleApiKey;

            if (ModelState.IsValid)
            {
                try
                {
                    var profile = _mapper.Map<RestaurantProfile>(model.Profile);
                    profile.UpdatedBy = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                    profile.UpdatedTime = System.DateTime.Now;

                    _restaurantProfileService.EditRestaurantProfile(profile);
                    TempData["SuccessMessage"] = "Address updated successfully.";
                    return RedirectToAction("RestaurantProfileForm");
                }
                catch (ArgumentException ex)
                {
                    _logger.LogWarning(ex, "Validation error updating restaurant address");
                    ModelState.AddModelError("", ex.Message);
                    TempData["ErrorMessage"] = ex.Message;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating restaurant address");
                    ModelState.AddModelError("", "An unexpected error occurred while updating the address.");
                    TempData["ErrorMessage"] = "An unexpected error occurred while updating the address.";
                }
            }

            return View("RestaurantProfileForm", model);
        }
    }
}
