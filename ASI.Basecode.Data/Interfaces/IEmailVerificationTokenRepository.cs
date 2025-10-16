using ASI.Basecode.Data.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IEmailVerificationTokenRepository
    {
        /// <summary>
        /// Creates a new email verification token
        /// </summary>
        Task<EmailVerificationToken> CreateAsync(EmailVerificationToken token);

        /// <summary>
        /// Gets the latest active token for a user
        /// </summary>
        EmailVerificationToken GetLatestTokenByUserId(int userId);

        /// <summary>
        /// Gets a token by its value
        /// </summary>
        EmailVerificationToken GetByToken(string token);

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
        IQueryable<EmailVerificationToken> GetTokensByUserId(int userId);

        /// <summary>
        /// Updates a token
        /// </summary>
        Task UpdateAsync(EmailVerificationToken token);
    }
}
