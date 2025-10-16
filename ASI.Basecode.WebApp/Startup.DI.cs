using ASI.Basecode.Data;
using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.ServiceModels;
using ASI.Basecode.Services.Services;
using ASI.Basecode.WebApp.Authentication;
using ASI.Basecode.WebApp.Models;
using ASI.Basecode.Data.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ASI.Basecode.WebApp
{
    // Other services configuration
    internal partial class StartupConfigurer
    {
        /// <summary>
        /// Configures the other services.
        /// </summary>
        private void ConfigureOtherServices()
        {
            // Framework
            this._services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            this._services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();

            // Common
            this._services.AddScoped<TokenProvider>();
            this._services.TryAddSingleton<TokenProviderOptionsFactory>();
            this._services.TryAddSingleton<TokenValidationParametersFactory>();
            this._services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Services
            this._services.TryAddSingleton<TokenValidationParametersFactory>();
            this._services.AddScoped<IUserService, UserService>();
            this._services.AddScoped<IMailSender, MailSenderService>();
            this._services.AddScoped<IOtpService, OtpService>();
            this._services.AddScoped<ICategoryService, CategoryService>();
            this._services.AddScoped<IFileStorageService, FileStorageService>();
            this._services.AddScoped<IFileHandlingService, FileHandlingService>();
            this._services.AddScoped<IProductService, ProductService>();
            this._services.AddScoped<IRestaurantProfileService, RestaurantProfileService>();
            this._services.AddScoped<IStaffService, StaffService>();

            this._services.AddScoped<ICartService, CartService>();
            this._services.AddScoped<IUserProfileService, UserProfileService>();
            this._services.AddScoped<IAddressService, AddressService>();

            this._services.AddScoped<IEmailVerificationTokenRepository, EmailVerificationTokenRepository>();
            this._services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
            this._services.AddHostedService<TokenCleanupService>();
            
            // Notification Service
            this._services.AddScoped<IOrderNotificationService, OrderNotificationService>();

            // Repositories
            this._services.AddScoped<IUserRepository, UserRepository>();
            this._services.AddScoped<ICategoryRepository, CategoryRepository>();
            this._services.AddScoped<IProductRepository, ProductRepository>();
            this._services.AddScoped<IRestaurantProfileRepository, RestaurantProfileRepository>();
            this._services.AddScoped<ICartRepository, CartRepository>();
            this._services.AddScoped<IUserProfileRepository, UserProfileRepository>();

            // Manager Class
            this._services.AddScoped<SignInManager>();

            // Filters
            this._services.AddScoped<AuthenticationUserFilters>();

            this._services.AddHttpClient();
        }
    }
}
