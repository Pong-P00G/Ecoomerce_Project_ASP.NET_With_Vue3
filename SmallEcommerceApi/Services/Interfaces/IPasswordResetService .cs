using SmallEcommerceApi.Models;

namespace SmallEcommerceApi.Services.Interfaces
{
    public interface IPasswordResetService
    {
        Task<PasswordResetToken> CreateTokenAsync(int userId, string token, DateTime expiresAt);
        Task<PasswordResetToken?> GetValidTokenAsync(string token);
        Task MarkAsUsedAsync(PasswordResetToken token);
    }

}
