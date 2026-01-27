using Microsoft.EntityFrameworkCore;
using SmallEcommerceApi.DTOs.Auth;
using SmallEcommerceApi.Models.Auth;
using static SmallEcommerceApi.DTOs.Auth.AuthDto;


namespace SmallEcommerceApi.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);
        Task<AuthResponseDto?> RefreshTokenAsync(string refreshToken);
        Task<bool> LogoutAsync(int userId);
        Task<bool> RevokeRefreshTokenAsync(string refreshToken);
    }
}