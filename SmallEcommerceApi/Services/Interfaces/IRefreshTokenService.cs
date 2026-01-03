using SmallEcommerceApi.Models;

namespace SmallEcommerceApi.Services.Interfaces
{
    public interface IRefreshTokenService
    {
        Task<RefreshToken> CreateTokenAsync(int userId, string token, DateTime expiresAt);
        Task<RefreshToken?> GetValidTokenAsync(string token);
        Task RevokeTokenAsync(RefreshToken token);
    }
}
