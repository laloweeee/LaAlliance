using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Repositories
{
    public class EmailVerificationTokenRepository : IEmailVerificationTokenRepository
    {
        private readonly AsiBasecodeDBContext _context;

        public EmailVerificationTokenRepository(AsiBasecodeDBContext context)
        {
            _context = context;
        }

        public async Task<EmailVerificationToken> CreateAsync(EmailVerificationToken token)
        {
            await _context.EmailVerificationTokens.AddAsync(token);
            await _context.SaveChangesAsync();
            return token;
        }

        public EmailVerificationToken GetByToken(string token)
        {
            return _context.EmailVerificationTokens
                .Include(t => t.User)
                .FirstOrDefault(t => t.Token == token);
        }

        public EmailVerificationToken GetLatestTokenByUserId(int userId)
        {
            return _context.EmailVerificationTokens
                .Where(t => t.UserID == userId)
                .OrderByDescending(t => t.CreatedAt)
                .FirstOrDefault();
        }

        public IQueryable<EmailVerificationToken> GetTokensByUserId(int userId)
        {
            return _context.EmailVerificationTokens
                .Where(t => t.UserID == userId)
                .OrderByDescending(t => t.CreatedAt)
                .AsQueryable();
        }

        public async Task MarkAsUsedAsync(int tokenId)
        {
            var token = await _context.EmailVerificationTokens.FindAsync(tokenId);
            if (token != null)
            {
                token.IsUsed = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteExpiredTokensAsync(DateTime expirationDate)
        {
            var expiredTokens = await _context.EmailVerificationTokens
                .Where(t => t.ExpiresAt < expirationDate)
                .ToListAsync();

            _context.EmailVerificationTokens.RemoveRange(expiredTokens);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(EmailVerificationToken token)
        {
            _context.EmailVerificationTokens.Update(token);
            await _context.SaveChangesAsync();
        }
    }
}
