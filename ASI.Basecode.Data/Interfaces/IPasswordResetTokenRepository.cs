using ASI.Basecode.Data.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IPasswordResetTokenRepository
    {
        /// <summary>
        /// Creates a new password reset token
        /// </summary>
        Task<PasswordResetToken> CreateAsync(PasswordResetToken token);

        /// <summary>
        /// Gets the latest active token for a user
        /// </summary>
        Task<PasswordResetToken> GetLatestTokenByUserId(int userId);

        /// <summary>
        /// Gets a token by its value
        /// </summary>
        Task<PasswordResetToken> GetByToken(string token);

        /// <summary>
        /// Marks a token as used
        /// </summary>
        Task MarkAsUsedAsync(int tokenId);

        /// <summary>
        /// Deletes expired tokens
        /// </summary>
        Task DeleteExpiredTokensAsync(DateTime expirationDate);

        /// <summary>
        /// Gets all tokens for a user
        /// </summary>
        IQueryable<PasswordResetToken> GetTokensByUserId(int userId);

        /// <summary>
        /// Updates a token
        /// </summary>
        Task UpdateAsync(PasswordResetToken token);
    }
}
