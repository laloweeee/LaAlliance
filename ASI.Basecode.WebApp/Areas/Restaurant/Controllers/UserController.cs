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
    public class UserController : ControllerBase<UserController>
    {
        private readonly IUserProfileService _userProfileService;
        private readonly IUserService _userService;

        public UserController(
            IHttpContextAccessor httpContextAccessor,
            ILoggerFactory loggerFactory,
            IConfiguration configuration,
            IMapper mapper,
            IUserProfileService userProfileService,
            IUserService userService
        ) : base(httpContextAccessor, loggerFactory, configuration, mapper) 
        {
            _userProfileService = userProfileService;
            _userService = userService;
        }

        public IActionResult Profile()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                
                // Check if user is Admin or Staff
                var userRole = User.FindFirst("RestaurantRole")?.Value;
                var isAdmin = userRole == "Admin";
                
                ViewBag.IsAdmin = isAdmin;
                ViewBag.UserRole = userRole;
                ViewBag.UserName = User.Identity?.Name ?? "User";
                
                // Get user profile (now properly handles both User and UserProfile tables)
                UserProfileServiceModel userProfile;
                try
                {
                    userProfile = _userProfileService.GetUserProfile(userId);
                }
                catch (Exception profileEx)
                {
                    _logger.LogWarning(profileEx, "Error getting user profile for user {UserId}, creating default profile", userId);
                    
                    // Create a default profile with basic user information
                    userProfile = new UserProfileServiceModel
                    {
                        UserID = userId,
                        Email = User.FindFirst(ClaimTypes.Email)?.Value ?? User.Identity?.Name ?? "",
                        FirstName = "User",
                        LastName = "Profile",
                        ContactNumber = ""
                    };
                }
                
                return View(userProfile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading user profile");
                TempData["ErrorMessage"] = "Error loading profile information.";
                return View(new UserProfileServiceModel());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateProfile(UserProfileServiceModel model)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var userRole = User.FindFirst("RestaurantRole")?.Value;
                
                // Only Admin can update profile
                if (userRole != "Admin")
                {
                    TempData["ErrorMessage"] = "You don't have permission to update profile.";
                    return RedirectToAction("Profile");
                }

                if (ModelState.IsValid)
                {
                    model.UserID = userId;
                    _userProfileService.UpdateUserProfile(model);
                    TempData["SuccessMessage"] = "Profile updated successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Please correct the errors and try again.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile");
                TempData["ErrorMessage"] = "Error updating profile.";
            }
            
            return RedirectToAction("Profile");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteProfile()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var userRole = User.FindFirst("RestaurantRole")?.Value;
                
                // Only Admin can delete profile
                if (userRole != "Admin")
                {
                    TempData["ErrorMessage"] = "You don't have permission to delete profile.";
                    return RedirectToAction("Profile");
                }

                // Delete the user and all related data
                _userService.DeleteUser(userId);
                
                // Sign out the user
                HttpContext.Session.Clear();
                
                // Redirect to login page
                TempData["SuccessMessage"] = "Your profile has been successfully deleted.";
                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user profile for user ID {UserId}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                TempData["ErrorMessage"] = "Error deleting profile. Please try again.";
                return RedirectToAction("Profile");
            }
        }

        public IActionResult Settings()
        {
            return View(); 
        }

        #region Password Management

        /// <summary>
        /// Displays the password change form using Restaurant layout
        /// </summary>
        /// <returns></returns>
        public IActionResult PasswordForm()
        {
            return View();
        }

        #endregion

        #region Email Management

        /// <summary>
        /// Displays the email change form using Restaurant layout
        /// </summary>
        /// <returns></returns>
        public IActionResult EmailForm()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var user = _userService.GetUserByID(userId);
                
                if (user == null)
                {
                    TempData["ErrorMessage"] = "User not found.";
                    return RedirectToAction("Profile");
                }

                var model = new ASI.Basecode.WebApp.Areas.Customer.Models.EmailViewModel
                {
                    CurrentEmail = user.Email
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading email form for user ID {UserId}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                TempData["ErrorMessage"] = "An error occurred while loading the email form.";
                return RedirectToAction("Profile");
            }
        }

        #endregion
    }
}