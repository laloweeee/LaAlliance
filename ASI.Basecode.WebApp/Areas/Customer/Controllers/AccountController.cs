using System;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.WebApp.Areas.Customer.Models;

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

        [HttpGet]
        public IActionResult Index()
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
                Addresses = _mapper.Map<System.Collections.Generic.List<UserAddressViewModel>>(userProfile.Addresses)
            };

            return View(model);
        }

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
    }
}