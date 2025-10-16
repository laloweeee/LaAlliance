using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using AutoMapper;
using ASI.Basecode.WebApp.Controllers;
using ASI.Basecode.WebApp.Authentication;
using ASI.Basecode.WebApp.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Resources.Constants;
using static ASI.Basecode.Resources.Constants.Enums;
using System.Threading.Tasks;

namespace ASI.Basecode.Tests.WebApp
{
    public class LoginTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<SignInManager> _signInManagerMock;
        private readonly Mock<IMailSender> _mailSenderMock;
        private readonly Mock<IOtpService> _otpServiceMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly AccountController _controller;

        public LoginTests()
        {
            // Initialize mocks
            _userServiceMock = new Mock<IUserService>();
            _mailSenderMock = new Mock<IMailSender>();
            _otpServiceMock = new Mock<IOtpService>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _configurationMock = new Mock<IConfiguration>();
            _mapperMock = new Mock<IMapper>();

            // Setup HttpContext with Session
            var sessionMock = new Mock<ISession>();
            var httpContext = new DefaultHttpContext();
            httpContext.Session = sessionMock.Object;
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            // Mock SignInManager
            _signInManagerMock = new Mock<SignInManager>(
                _configurationMock.Object,
                _httpContextAccessorMock.Object
            );

            // Create mocks for TokenValidationParametersFactory and TokenProviderOptionsFactory
            var tokenValidationFactoryMock = new Mock<TokenValidationParametersFactory>(_configurationMock.Object);
            var tokenProviderFactoryMock = new Mock<TokenProviderOptionsFactory>(_configurationMock.Object);

            // Create controller instance
            _controller = new AccountController(
                _signInManagerMock.Object,
                _httpContextAccessorMock.Object,
                Mock.Of<ILoggerFactory>(),
                _configurationMock.Object,
                _mapperMock.Object,
                _userServiceMock.Object,
                _mailSenderMock.Object,
                _otpServiceMock.Object,
                tokenValidationFactoryMock.Object,
                tokenProviderFactoryMock.Object
            );

            // Set controller context
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Setup TempData
            var tempData = new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(
                httpContext,
                Mock.Of<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider>()
            );
            _controller.TempData = tempData;
        }

        [Fact]
        public async Task Login_Post_InvalidCredentials_ReturnsViewWithError()
        {
            // Arrange
            var model = new LoginViewModel
            {
                Email = "wrong@example.com",
                Password = "wrongpass"
            };

            User? outUser = null;
            _userServiceMock
                .Setup(s => s.AuthenticateUser(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    ref outUser))
                .Returns(LoginResult.Failed);

            // Act
            var result = await _controller.Login(model, null) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(_controller.TempData["ErrorMessage"]);
            Assert.Equal("Incorrect Email or Password", _controller.TempData["ErrorMessage"]);
        }

        [Fact]
        public async Task Login_Post_ValidCustomer_RedirectsToCustomerHome()
        {
            // Arrange
            var model = new LoginViewModel
            {
                Email = "customer@example.com",
                Password = "Password123"
            };

            var user = new User
            {
                UserID = 1,
                Email = model.Email,
                UserType = UserType.Customer,
                UserProfile = new UserProfile
                {
                    FirstName = "John",
                    LastName = "Doe"
                }
            };

            User outUser = user;
            _userServiceMock
                .Setup(s => s.AuthenticateUser(
                    model.Email,
                    model.Password,
                    ref outUser))
                .Returns(LoginResult.Success);

            _signInManagerMock
                .Setup(s => s.SignInAsync(It.IsAny<User>(), It.IsAny<bool>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Login(model, null) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            Assert.Equal("Home", result.ControllerName);
            Assert.Equal("Customer", result.RouteValues["area"]);
        }

        [Fact]
        public async Task Login_Post_ValidRestaurant_RedirectsToRestaurantDashboard()
        {
            // Arrange
            var model = new LoginViewModel
            {
                Email = "restaurant@example.com",
                Password = "Password123"
            };

            var user = new User
            {
                UserID = 2,
                Email = model.Email,
                UserType = UserType.Restaurant,
                RestaurantStaff = new RestaurantStaff
                {
                    StaffID = 1,
                    Role = Enums.StaffRole.Admin
                },
                UserProfile = new UserProfile
                {
                    FirstName = "Jane",
                    LastName = "Smith"
                }
            };

            User outUser = user;
            _userServiceMock
                .Setup(s => s.AuthenticateUser(
                    model.Email,
                    model.Password,
                    ref outUser))
                .Returns(LoginResult.Success);

            _signInManagerMock
                .Setup(s => s.SignInAsync(It.IsAny<User>(), It.IsAny<bool>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Login(model, null) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.ActionName);
            Assert.NotNull(result.ControllerName);
            Assert.Equal("Index", result.ActionName);
            Assert.Equal("Dashboard", result.ControllerName);
            Assert.Equal("Restaurant", result.RouteValues["area"]);
        }

        [Fact]
        public async Task Login_Post_RestaurantWithoutStaff_LoadsStaffAndRedirects()
        {
            // Arrange
            var model = new LoginViewModel
            {
                Email = "restaurant@example.com",
                Password = "Password123"
            };

            var userWithoutStaff = new User
            {
                UserID = 3,
                Email = model.Email,
                UserType = UserType.Restaurant,
                RestaurantStaff = null
            };

            var userWithStaff = new User
            {
                UserID = 3,
                Email = model.Email,
                UserType = UserType.Restaurant,
                RestaurantStaff = new RestaurantStaff
                {
                    StaffID = 2,
                    Role = Enums.StaffRole.Staff
                }
            };

            User outUser = userWithoutStaff;
            _userServiceMock
                .Setup(s => s.AuthenticateUser(
                    model.Email,
                    model.Password,
                    ref outUser))
                .Returns(LoginResult.Success);

            _userServiceMock
                .Setup(s => s.GetUserByID(userWithoutStaff.UserID))
                .Returns(userWithStaff);

            _signInManagerMock
                .Setup(s => s.SignInAsync(It.IsAny<User>(), It.IsAny<bool>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Login(model, null) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            _userServiceMock.Verify(s => s.GetUserByID(userWithoutStaff.UserID), Times.Once);
        }

        [Fact]
        public async Task Login_Post_NullUser_ReturnsViewWithError()
        {
            // Arrange
            var model = new LoginViewModel
            {
                Email = "test@example.com",
                Password = "Password123"
            };

            User? outUser = null;
            _userServiceMock
                .Setup(s => s.AuthenticateUser(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    ref outUser))
                .Returns(LoginResult.Success);

            // Act
            var result = await _controller.Login(model, null) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(_controller.TempData["ErrorMessage"]);
            Assert.Equal("Incorrect Email or Password", _controller.TempData["ErrorMessage"]);
        }
    }
}