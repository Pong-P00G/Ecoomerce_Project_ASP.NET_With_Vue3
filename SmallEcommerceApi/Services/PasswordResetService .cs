using Microsoft.EntityFrameworkCore;
using SmallEcommerceApi.Db;
using SmallEcommerceApi.Models;
using SmallEcommerceApi.Services.Interfaces;

namespace SmallEcommerceApi.Services
{
    public class PasswordResetService : IPasswordResetService
    {
        private readonly AppDbContext _db;
        public PasswordResetService(AppDbContext db) => _db = db;

        public async Task<PasswordResetToken> CreateTokenAsync(int userId, string token, DateTime expiresAt)
        {
            var user = await _db.Users.FindAsync(userId) ?? throw new ArgumentException($"User with ID {userId} not found.", nameof(userId));
            PasswordResetToken resetToken = new PasswordResetToken
            {
                UserId = userId,
                Token = token,
                ExpiresAt = expiresAt,
                User = user
            };
            _db.PasswordResetTokens.Add(resetToken);
            await _db.SaveChangesAsync();
            return resetToken;
        }

        public async Task<PasswordResetToken?> GetValidTokenAsync(string token) =>
            await _db.PasswordResetTokens.FirstOrDefaultAsync(t =>
                t.Token == token && !t.IsUsed && t.ExpiresAt > DateTime.UtcNow);

        public async Task MarkAsUsedAsync(PasswordResetToken token)
        {
            token.IsUsed = true;
            await _db.SaveChangesAsync();
        }
    }
}
