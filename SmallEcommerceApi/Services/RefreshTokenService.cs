using Microsoft.EntityFrameworkCore;
using SmallEcommerceApi.Db;
using SmallEcommerceApi.Models;
using SmallEcommerceApi.Services.Interfaces;

namespace SmallEcommerceApi.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly AppDbContext _db;
        public RefreshTokenService(AppDbContext db) => _db = db;

        public async Task<RefreshToken> CreateTokenAsync(int userId, string token, DateTime expiresAt)
        {
            var user = await _db.Users.FindAsync(userId);
            if (user == null)
                throw new InvalidOperationException($"User with ID {userId} not found.");

            var refresh = new RefreshToken
            {
                UserId = userId,
                Token = token,
                ExpirationDate = expiresAt,
                User = user
            };
            _db.RefreshTokens.Add(refresh);
            await _db.SaveChangesAsync();
            return refresh;
        }

        public async Task<RefreshToken?> GetValidTokenAsync(string token) =>
            await _db.RefreshTokens.FirstOrDefaultAsync(t =>
                t.Token == token && !t.IsRevoked && t.ExpiresAt > DateTime.UtcNow);

        public async Task RevokeTokenAsync(RefreshToken token)
        {
            token.IsRevoked = true;
            token.RevokedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }
    }
}
