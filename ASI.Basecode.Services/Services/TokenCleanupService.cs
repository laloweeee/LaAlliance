using ASI.Basecode.Data.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Services
{
    /// <summary>
    /// Background service to clean up expired tokens
    /// </summary>
    public class TokenCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TokenCleanupService> _logger;
        private readonly TimeSpan _cleanupInterval = TimeSpan.FromHours(24); // Run daily
        private readonly int _tokenRetentionDays = 7; // Keep tokens for 7 days after expiration

        public TokenCleanupService(IServiceProvider serviceProvider, ILogger<TokenCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Token Cleanup Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CleanupExpiredTokens();
                    await Task.Delay(_cleanupInterval, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during token cleanup");
                    // Wait 1 hour before retrying if there's an error
                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                }
            }

            _logger.LogInformation("Token Cleanup Service stopped");
        }

        private async Task CleanupExpiredTokens()
        {
            using var scope = _serviceProvider.CreateScope();
            
            var emailTokenRepository = scope.ServiceProvider.GetRequiredService<IEmailVerificationTokenRepository>();
            var passwordResetTokenRepository = scope.ServiceProvider.GetRequiredService<IPasswordResetTokenRepository>();

            var expirationDate = DateTime.UtcNow.AddDays(-_tokenRetentionDays);

            try
            {
                // Clean up email verification tokens
                await emailTokenRepository.DeleteExpiredTokensAsync(expirationDate);
                
                // Clean up password reset tokens
                await passwordResetTokenRepository.DeleteExpiredTokensAsync(expirationDate);

                _logger.LogInformation($"Token cleanup completed successfully at {DateTime.UtcNow}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to cleanup expired tokens");
                throw;
            }
        }
    }
}
