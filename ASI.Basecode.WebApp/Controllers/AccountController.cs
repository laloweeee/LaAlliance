using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.Manager;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.WebApp.Authentication;
using ASI.Basecode.WebApp.Models;
using ASI.Basecode.WebApp.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static ASI.Basecode.Resources.Constants.Enums;

namespace ASI.Basecode.WebApp.Controllers
{
    public class AccountController : ControllerBase<AccountController>
    {
        private readonly SessionManager _sessionManager;
        private readonly SignInManager _signInManager;
        private readonly TokenValidationParametersFactory _tokenValidationParametersFactory;
        private readonly TokenProviderOptionsFactory _tokenProviderOptionsFactory;
        private readonly IConfiguration _appConfiguration;
        private readonly IUserService _userService;
        private readonly IMailSender _mailSender;
        private readonly IOtpService _otpService;
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController"/> class.
        /// </summary>
        /// <param name="signInManager">The sign in manager.</param>
        /// <param name="localizer">The localizer.</param>
        /// <param name="userService">The user service.</param>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="tokenValidationParametersFactory">The token validation parameters factory.</param>
        /// <param name="tokenProviderOptionsFactory">The token provider options factory.</param>
        public AccountController(
                            SignInManager signInManager,
                            IHttpContextAccessor httpContextAccessor,
                            ILoggerFactory loggerFactory,
                            IConfiguration configuration,
                            IMapper mapper,
                            IUserService userService,
                            IMailSender mailSender,
                            IOtpService otpService,
                            TokenValidationParametersFactory tokenValidationParametersFactory,
                            TokenProviderOptionsFactory tokenProviderOptionsFactory) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            this._sessionManager = new SessionManager(this._session);
            this._signInManager = signInManager;
            this._tokenProviderOptionsFactory = tokenProviderOptionsFactory;
            this._tokenValidationParametersFactory = tokenValidationParametersFactory;
            this._appConfiguration = configuration;
            this._userService = userService;
            this._mailSender = mailSender;
            this._otpService = otpService;
        }

        /// <summary>
        /// Login Method
        /// </summary>
        /// <returns>Created response view</returns>
        [HttpGet]
        [AllowAnonymous]
        [ServiceFilter(typeof(AuthenticationUserFilters))]
        public ActionResult Login()
        {
            TempData["returnUrl"] = System.Net.WebUtility.UrlDecode(HttpContext.Request.Query["ReturnUrl"]);
            this._sessionManager.Clear();
            this._session.SetString("SessionId", System.Guid.NewGuid().ToString());
            return this.View();
        }

        /// <summary>
        /// Authenticate user and signs the user in when successful.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns> Created response view </returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            this._session.SetString("HasSession", "Exist");

            User user = null;

            var loginResult = _userService.AuthenticateUser(model.Email, model.Password, ref user);

            if (loginResult == LoginResult.Success && user != null)
            {
                if (user.UserType == UserType.Restaurant && user.RestaurantStaff == null)
                {
                    user = _userService.GetUserByID(user.UserID);
                }

                await this._signInManager.SignInAsync(user);
                this._session.SetString("UserEmail", model.Email);

                return user.UserType switch
                {
                    UserType.Customer => RedirectToAction("Index", "Home", new { area = "Customer" }),
                    UserType.Restaurant => RedirectToAction("Index", "Dashboard", new { area = "Restaurant" }),
                    _ => RedirectToAction("Login", "Account"),
                };
            }
            else
            {
                TempData["ErrorMessage"] = "Incorrect Email or Password";
                return View();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [ServiceFilter(typeof(AuthenticationUserFilters))]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(UserViewModel model)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                
                _userService.AddUser(model);

                var otpCreated = await _otpService.CreateOtpForUser(model.Email);

                if (!otpCreated)
                {
                    TempData["ErrorMessage"] = "Failed to create OTP.";
                    return View(model);
                }

                TempData["SuccessMessage"] = "Registration successful! Please check your email for verification code.";

                return RedirectToAction("Verification", "Account", new { email = model.Email });
            }
            catch (InvalidDataException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration");
                TempData["ErrorMessage"] = Resources.Messages.Errors.ServerError;
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Verification(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Register");
            }

            var model = new EmailVerificationModel { Email = email };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyOtp(EmailVerificationModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("Verification", model);
                }

                if (_otpService.VerifyOTP(model.Email, int.Parse(model.OtpCode)))
                {
                    await _otpService.MarkVerified(model.Email);
                    TempData["SuccessMessage"] = "Email verified successfully.";
                    return RedirectToAction("Login", "Account");
                }

                TempData["ErrorMessage"] = "Invalid or expired OTP. Please try again.";
                return View("Verification", model);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred during verification.";
                return View("Verification", model);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ResendCode(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var otpCreated = await _otpService.CreateOtpForUser(email);

                if (!otpCreated)
                {
                    TempData["ErrorMessage"] = "Failed to create OTP.";
                    return View();
                }

                TempData["SuccessMessage"] = "Verification code resent. Please check your email.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resending verification code");
                TempData["ErrorMessage"] = "Failed to resend verification code.";
            }

            return RedirectToAction("Verification", new { email });
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        /// <summary>
        /// Sign Out current account and return login view.
        /// </summary>
        /// <returns>Created response view</returns>
        [AllowAnonymous]
        public async Task<IActionResult> SignOutUser()
        {
            await this._signInManager.SignOutAsync();
            return Redirect("/");
        }

        /// <summary>
        /// Alias endpoint for logout used by views.
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await this._signInManager.SignOutAsync();
            return Redirect("/");
        }

        /// <summary>
        /// Authenticate user and signs the user in when successful.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="returnUrl">The return URL.</param>
        /// <returns> Created response view </returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            this._session.SetString("HasSession", "Exist");

            User user = null;

            var loginResult = _userService.AuthenticateUser(model.Email, model.Password, ref user);

            if (loginResult == LoginResult.Success && user != null)
            {
                // Check account status before allowing login
                
                // Check if account is disabled
                if (user.AccountStatus == AccountStatus.Disabled)
                {
                    TempData["ErrorMessage"] = "This account has been disabled. Please contact support for assistance.";
                    return View();
                }
                
                // Check if email is verified
                if (!user.IsEmailVerified)
                {
                    TempData["ErrorMessage"] = "Please verify your email before logging in.";
                    return View();
                }

                // Load RestaurantStaff if needed
                if (user.UserType == UserType.Restaurant && user.RestaurantStaff == null)
                {
                    user = _userService.GetUserByID(user.UserID);
                }
                
                await this._signInManager.SignInAsync(user);
                this._session.SetString("UserEmail", model.Email);

                return user.UserType switch
                {
                    UserType.Customer => RedirectToAction("Index", "Home", new { area = "Customer" }),
                    UserType.Restaurant => RedirectToAction("Index", "Dashboard", new { area = "Restaurant" }),
                    _ => RedirectToAction("Login", "Account"),
                };
            }
            else
            {
                TempData["ErrorMessage"] = "Incorrect Email or Password";
                return View();
            }
        }
    }
}
