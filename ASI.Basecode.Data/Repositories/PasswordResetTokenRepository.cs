using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Repositories
{
    public class PasswordResetTokenRepository : BaseRepository, IPasswordResetTokenRepository
    {
        private readonly AsiBasecodeDBContext _dbContext;

        public PasswordResetTokenRepository(IUnitOfWork unitOfWork, AsiBasecodeDBContext dbContext) : base(unitOfWork)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Creates a new password reset token
        /// </summary>
        public async Task<PasswordResetToken> CreateAsync(PasswordResetToken token)
        {
            await _dbContext.PasswordResetTokens.AddAsync(token);
            await _dbContext.SaveChangesAsync();
            return token;
        }

        /// <summary>
        /// Gets the latest active token for a user
        /// </summary>
        public async Task<PasswordResetToken> GetLatestTokenByUserId(int userId)
        {
            return await _dbContext.PasswordResetTokens
                .Where(t => t.UserID == userId && !t.IsUsed)
                .OrderByDescending(t => t.CreatedAt)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets a token by its value
        /// </summary>
        public async Task<PasswordResetToken> GetByToken(string token)
        {
            return await _dbContext.PasswordResetTokens
                .FirstOrDefaultAsync(t => t.Token == token && !t.IsUsed);
        }

        /// <summary>
        /// Marks a token as used
        /// </summary>
        public async Task MarkAsUsedAsync(int tokenId)
        {
            var token = await _dbContext.PasswordResetTokens.FindAsync(tokenId);
            if (token != null)
            {
                token.IsUsed = true;
                await _dbContext.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Deletes expired tokens
        /// </summary>
        public async Task DeleteExpiredTokensAsync(DateTime expirationDate)
        {
            var expiredTokens = _dbContext.PasswordResetTokens
                .Where(t => t.ExpiresAt < expirationDate);
            
            _dbContext.PasswordResetTokens.RemoveRange(expiredTokens);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Gets all tokens for a user
        /// </summary>
        public IQueryable<PasswordResetToken> GetTokensByUserId(int userId)
        {
            return _dbContext.PasswordResetTokens
                .Where(t => t.UserID == userId)
                .AsQueryable();
        }

        /// <summary>
        /// Updates a token
        /// </summary>
        public async Task UpdateAsync(PasswordResetToken token)
        {
            _dbContext.PasswordResetTokens.Update(token);
            await _dbContext.SaveChangesAsync();
        }
    }
}
