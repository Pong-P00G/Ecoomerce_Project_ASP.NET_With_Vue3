using SmallEcommerceApi.Models;
using SmallEcommerceApi.Models.Auth;
using SmallEcommerceApi.Models.Users;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;


namespace SmallEcommerceApi.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        ClaimsPrincipal? ValidateToken(string token);
    }
}
