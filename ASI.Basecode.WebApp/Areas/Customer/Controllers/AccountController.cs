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
using System.IO;

namespace ASI.Basecode.WebApp.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class AccountController : ControllerBase<AccountController>
    {
        private readonly IUserService _userService;
        private readonly IUserProfileService _userProfileService;
        private readonly IAddressService _addressService;
        private readonly IOtpService _otpService;

        public AccountController(
                    IHttpContextAccessor httpContextAccessor,
                    ILoggerFactory loggerFactory,
                    IConfiguration configuration,
                    IUserService userService,
                    IUserProfileService userProfileService,
                    IAddressService addressService,
                    IOtpService otpService,
                    IMapper mapper = null,
                    ICartService cartService = null)
                    : base(httpContextAccessor, loggerFactory, configuration, mapper, cartService
        )
        {
            _userService = userService;
            _addressService = addressService;
            _userProfileService = userProfileService;
            _otpService = otpService;
        }

        /// <summary>
        /// Displays the account overview page.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            var user = _userService.GetUserByID(UserId);
            var address = _addressService.GetUserAddresses(UserId);

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

        #region Address Management

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
                var addresses = _addressService.GetUserAddresses(UserId);
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
                    _addressService.UpdateUserAddress(userAddress, UserId);
                    TempData["SuccessMessage"] = "Address updated successfully.";
                }
                else
                {
                    _addressService.AddUserAddress(userAddress);
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
        public IActionResult DeleteAddress(int id)
        {
            try
            {
                _addressService.RemoveUserAddress(id, UserId);
                TempData["SuccessMessage"] = "Address deleted successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting address ID {AddressId} for user ID {UserId}", id, UserId);
                TempData["ErrorMessage"] = "An error occurred while deleting the address. Please try again.";
            }

            return RedirectToAction("Index");
        }

        #endregion

        #region Password Management

        

        /// <summary>
        /// Displays the password change form.
        /// </summary>
        /// <returns></returns>
        public IActionResult PasswordForm()
        {
            return View();
        }

        /// <summary>
        /// Changes the user's password.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(PasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("PasswordForm", model);
            }

            if (TempData != null)
            {
                TempData.Remove("ErrorMessage");
                TempData.Remove("SuccessMessage");
            }

            if (model.NewPassword == model.CurrentPassword)
            {
                TempData["ErrorMessage"] = "The new password must be different from the current password.";
                return View("PasswordForm", model);
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                TempData["ErrorMessage"] = "The new password and confirmation password do not match.";
                return View("PasswordForm", model);
            }


            try
            {
                // Verify current password
                if (!_userService.VerifyPassword(UserId, model.CurrentPassword))
                {
                    return View("PasswordForm", model);
                }

                _userService.UpdatePassword(UserId, model.NewPassword);
                TempData["SuccessMessage"] = "Password changed successfully.";
                return RedirectToAction("Index");
            }
            catch (InvalidDataException ex)
            {
                _logger.LogError(ex, "Error changing password for user ID {UserId}", UserId);
                TempData["ErrorMessage"] = ex.Message;
                return View("PasswordForm", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for user ID {UserId}", UserId);
                TempData["ErrorMessage"] = "An error occurred while changing your password. Please try again.";
                return View("PasswordForm", model);
            }
        }

        /// <summary>
        /// Sends OTP to user's email for password reset verification
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendPasswordOTP()
        {
            try
            {
                var user = _userService.GetUserByID(UserId);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "User not found.";
                    return RedirectToAction("Login", "Auth", new { area = "Customer" });
                }

                var result = _otpService.CreatePasswordResetOtp(user.Email).Result;
                if (result)
                {
                    TempData["Email"] = user.Email;
                    TempData["VerificationType"] = "password";
                    return RedirectToAction("OTPForm");
                }

                TempData["ErrorMessage"] = "Failed to send OTP. Please try again.";
                return RedirectToAction("PasswordForm");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending password OTP for user ID {UserId}", UserId);
                TempData["ErrorMessage"] = "An error occurred. Please try again.";
                return RedirectToAction("PasswordForm");
            }
        }

        /// <summary>
        /// Displays OTP verification form
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult OTPForm()
        {
            if (TempData["Email"] == null || TempData["VerificationType"] == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.Email = TempData["Email"];
            ViewBag.VerificationType = TempData["VerificationType"];
            ViewBag.NewEmail = TempData["NewEmail"];

            // Keep the data for the POST
            TempData.Keep("Email");
            TempData.Keep("VerificationType");
            TempData.Keep("NewEmail");

            return View();
        }

        /// <summary>
        /// Verifies OTP for password or email change
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult VerifyOTP(OTPVerificationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid OTP format.";
                return RedirectToAction("OTPForm");
            }

            var email = TempData["Email"]?.ToString();
            var verificationType = TempData["VerificationType"]?.ToString();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(verificationType))
            {
                TempData["ErrorMessage"] = "Session expired. Please try again.";
                return RedirectToAction("Index");
            }

            try
            {
                if (!int.TryParse(model.OTPCode, out int otpCode))
                {
                    TempData["ErrorMessage"] = "Invalid OTP format.";
                    TempData["Email"] = email;
                    TempData["VerificationType"] = verificationType;
                    return RedirectToAction("OTPForm");
                }

                bool isValid = false;

                if (verificationType == "password")
                {
                    isValid = _otpService.VerifyPasswordResetOTP(email, otpCode);
                    if (isValid)
                    {
                        _otpService.ClearPasswordResetOtp(email);
                        TempData["OTPVerified"] = true;
                        TempData["SuccessMessage"] = "OTP verified successfully. You can now change your password.";
                        return RedirectToAction("PasswordForm");
                    }
                }
                else if (verificationType == "email")
                {
                    isValid = _otpService.VerifyEmailChangeOTP(email, otpCode);
                    if (isValid)
                    {
                        _otpService.ClearEmailChangeOtp(email);
                        TempData["CurrentEmailVerified"] = true;
                        TempData["SuccessMessage"] = "Current email verified. Please enter your new email.";
                        return RedirectToAction("EmailForm");
                    }
                }
                else if (verificationType == "new-email")
                {
                    var newEmail = TempData["NewEmail"]?.ToString();
                    if (string.IsNullOrEmpty(newEmail))
                    {
                        TempData["ErrorMessage"] = "Session expired. Please try again.";
                        return RedirectToAction("EmailForm");
                    }

                    isValid = _otpService.VerifyEmailChangeOTP(newEmail, otpCode);
                    if (isValid)
                    {
                        _otpService.ClearEmailChangeOtp(newEmail);
                        _userService.UpdateEmail(UserId, newEmail);
                        TempData["SuccessMessage"] = "Email changed successfully.";
                        return RedirectToAction("Index");
                    }
                }

                TempData["ErrorMessage"] = "Invalid or expired OTP. Please try again.";
                TempData["Email"] = email;
                TempData["VerificationType"] = verificationType;
                return RedirectToAction("OTPForm");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying OTP for user ID {UserId}", UserId);
                TempData["ErrorMessage"] = "An error occurred. Please try again.";
                return RedirectToAction("OTPForm");
            }
        }

        #endregion

        #region Email Management

        /// <summary>
        /// Displays the email change form
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult EmailForm()
        {
            var user = _userService.GetUserByID(UserId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("Login", "Auth", new { area = "Customer" });
            }

            var model = new EmailViewModel
            {
                CurrentEmail = user.Email
            };

            return View(model);
        }

        /// <summary>
        /// Sends OTP to current email for verification
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendEmailOTP(EmailViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("EmailForm", model);
            }

            try
            {
                var user = _userService.GetUserByID(UserId);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "User not found.";
                    return RedirectToAction("Login", "Auth", new { area = "Customer" });
                }

                // Verify current email matches
                if (user.Email != model.CurrentEmail)
                {
                    TempData["ErrorMessage"] = "Current email does not match.";
                    return View("EmailForm", model);
                }

                // Check if new email is available
                if (!_userService.IsEmailAvailable(model.NewEmail))
                {
                    TempData["ErrorMessage"] = "This email is already in use.";
                    return View("EmailForm", model);
                }

                // Send OTP to current email first
                var result = _otpService.CreateEmailChangeOtp(user.Email).Result;
                if (result)
                {
                    TempData["Email"] = user.Email;
                    TempData["NewEmail"] = model.NewEmail;
                    TempData["VerificationType"] = "email";
                    return RedirectToAction("OTPForm");
                }

                TempData["ErrorMessage"] = "Failed to send OTP. Please try again.";
                return View("EmailForm", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email change OTP for user ID {UserId}", UserId);
                TempData["ErrorMessage"] = "An error occurred. Please try again.";
                return View("EmailForm", model);
            }
        }

        /// <summary>
        /// Sends OTP to new email for verification
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendNewEmailOTP()
        {
            try
            {
                var newEmail = TempData["NewEmail"]?.ToString();
                if (string.IsNullOrEmpty(newEmail))
                {
                    TempData["ErrorMessage"] = "Session expired. Please try again.";
                    return RedirectToAction("EmailForm");
                }

                var result = _otpService.CreateEmailChangeOtp(newEmail).Result;
                if (result)
                {
                    TempData["Email"] = newEmail;
                    TempData["VerificationType"] = "new-email";
                    TempData.Keep("NewEmail");
                    return RedirectToAction("OTPForm");
                }

                TempData["ErrorMessage"] = "Failed to send OTP. Please try again.";
                return RedirectToAction("EmailForm");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending new email OTP for user ID {UserId}", UserId);
                TempData["ErrorMessage"] = "An error occurred. Please try again.";
                return RedirectToAction("EmailForm");
            }
        }

        #endregion
    }
}