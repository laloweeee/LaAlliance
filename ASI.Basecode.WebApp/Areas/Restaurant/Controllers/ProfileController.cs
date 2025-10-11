using ASI.Basecode.Data.Models;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
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
            var model = _mapper.Map<RestaurantViewModel>(profile);
            var googleApiKey = _configuration["GoogleMaps:ApiKey"];

            ViewBag.GoogleMapsApiKey = googleApiKey;
            return View(model);
        }

        [HttpGet]
        public IActionResult RestaurantProfileForm()
        {
            var googleApiKey = _configuration["GoogleMaps:ApiKey"];
            ViewBag.GoogleMapsApiKey = googleApiKey;

            var restaurantData = _restaurantProfileService.GetRestaurantProfile();
            var profileViewModel = _mapper.Map<RestaurantViewModel>(restaurantData);
            return View(profileViewModel);
        }

        [HttpPost]
        public IActionResult UpdateRestaurantProfile(RestaurantViewModel model)
        {
            var googleApiKey = _configuration["GoogleMaps:ApiKey"];
            ViewBag.GoogleMapsApiKey = googleApiKey;

            if (ModelState.IsValid)
            {
                try
                {
                    var restaurant = _mapper.Map<Data.Models.Restaurant>(model);

                    _restaurantProfileService.EditRestaurantInformation(restaurant);
                    TempData["SuccessMessage"] = "Restaurant information updated successfully.";
                    return RedirectToAction("RestaurantProfileForm");
                }
                catch (ArgumentException ex)
                {
                    _logger.LogWarning(ex, "Validation error updating restaurant profile");
                    ModelState.AddModelError("", ex.Message);
                    TempData["ErrorMessage"] = ex.Message;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating restaurant profile");
                    ModelState.AddModelError("", "An unexpected error occurred while updating the profile.");
                    TempData["ErrorMessage"] = "An unexpected error occurred while updating the profile.";
                }
            }

            return View("RestaurantProfileForm", model);
        }
    }
}