using System;
using System.Linq;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.WebApp.Areas.Customer.Models;
using ASI.Basecode.Services.ServiceModels;

namespace ASI.Basecode.WebApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class AccountController : ControllerBase<AccountController>
    {
        private readonly IUserService _userService;
        private readonly IUserProfileService _userProfileService;
        public AccountController(
                    IHttpContextAccessor httpContextAccessor,
                    ILoggerFactory loggerFactory,
                    IConfiguration configuration,
                    IUserService userService,
                    IUserProfileService userProfileService,
                    IMapper mapper = null,
                    ICartService cartService = null)
                    : base(httpContextAccessor, loggerFactory, configuration, mapper, cartService
        )
        {
            _userService = userService;
            _userProfileService = userProfileService;
        }

        /// <summary>
        /// Displays the account overview page.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            var user = _userService.GetUserByID(UserId);
            var address = _userProfileService.GetUserAddresses(UserId);

            if (user == null)
            {
                return RedirectToAction("Login", "Auth", new { area = "Customer" });
            }

            var userProfile = _userProfileService.GetUserProfile(UserId);

            var model = new AccountViewModel
            {
                Email = user.Email,
                FirstName = userProfile.FirstName,
                LastName = userProfile.LastName,
                ContactNumber = userProfile.ContactNumber,
                Addresses = _mapper.Map<System.Collections.Generic.List<UserAddressViewModel>>(address)
            };

            _logger.LogInformation("User Profile: {@UserProfile}", model);

            return View(model);
        }

        /// <summary>
        /// Displays the profile form for viewing or editing user profile.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult ProfileForm()
        {
            var user = _userService.GetUserByID(UserId);

            if (user == null)
            {
                return RedirectToAction("Login", "Auth", new { area = "Customer" });
            }

            var userProfile = _userProfileService.GetUserProfile(UserId);

            var model = new AccountViewModel
            {
                Email = user.Email,
                FirstName = userProfile.FirstName,
                LastName = userProfile.LastName,
                ContactNumber = userProfile.ContactNumber,
            };

            return View(model);
        }


        /// <summary>
        /// Updates the user profile.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateProfile(AccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("ProfileForm", model);
            }

            try
            {
                // Fetch existing profile
                var userProfile = _userProfileService.GetUserProfile(UserId);

                // Update fields
                userProfile.FirstName = model.FirstName;
                userProfile.LastName = model.LastName;
                userProfile.ContactNumber = model.ContactNumber;

                // Save changes
                _userProfileService.UpdateUserProfile(userProfile);

                TempData["SuccessMessage"] = "Profile updated successfully.";

                // Redirect to avoid form resubmission
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile for user ID {UserId}", UserId);

                TempData["ErrorMessage"] = "An error occurred while updating your profile. Please try again.";

                return View("ProfileForm", model);
            }
        }

        /// <summary>
        /// Displays the address form for adding or editing an address.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult AddressForm(int? id)
        {
            /// Load Google Maps API key from configuration
            var googleApiKey = _configuration["GoogleMaps:ApiKey"];
            ViewBag.GoogleMapsApiKey = googleApiKey;

            _logger.LogInformation("Google Maps API Key: {ApiKey}", googleApiKey);

            // If id is provided, load the existing address for editing
            if (id.HasValue)
            {
                var addresses = _userProfileService.GetUserAddresses(UserId);
                var address = addresses.FirstOrDefault(a => a.UserAddressID == id.Value);

                if (address != null)
                {
                    var model = _mapper.Map<UserAddressViewModel>(address);
                    return View(model);
                }
                else
                {
                    TempData["ErrorMessage"] = "Address not found.";
                    return RedirectToAction("Index");
                }
            }

            return View();
        }

        /// <summary>
        /// Saves the user address (add or update).
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveAddress(UserAddressViewModel model)
        {
            try
            {
                var userAddress = new UserAddressServiceModel
                {
                    UserID = UserId,
                    AddressID = model.AddressID,
                    UserAddressID = model.UserAddressID,
                    IsDefault = model.IsDefault,
                    AddressType = model.AddressType,
                    AddressNote = model.AddressNote,
                    Street = model.Street,
                    Barangay = model.Barangay,
                    City = model.City,
                    Province = model.Province,
                    ZipCode = model.ZipCode,
                    Longitude = model.Longitude,
                    Latitude = model.Latitude,
                };

                // Check if AddressID is present to determine if it's an update or new address
                if (model.AddressID > 0)
                {
                    _userProfileService.UpdateUserAddress(userAddress, UserId);
                    TempData["SuccessMessage"] = "Address updated successfully.";
                }
                else
                {
                    _userProfileService.AddUserAddress(userAddress);
                    TempData["SuccessMessage"] = "Address added successfully.";
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving address for user ID {UserId}", UserId);

                TempData["ErrorMessage"] = "An error occurred while saving your address. Please try again.";

                return View("AddressForm", model);
            }
        }

        /// <summary>
        /// Deletes the specified user address.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteAddress(int addressID)
        {
            try
            {
                _userProfileService.RemoveUserAddress(addressID);
                TempData["SuccessMessage"] = "Address deleted successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting address ID {AddressId} for user ID {UserId}", addressID, UserId);
                TempData["ErrorMessage"] = "An error occurred while deleting the address. Please try again.";
            }

            return RedirectToAction("Index");
        }
    }
}