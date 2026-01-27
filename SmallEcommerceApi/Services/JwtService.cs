using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SmallEcommerceApi.Models.Users;
using SmallEcommerceApi.Security.Api.Security;
using SmallEcommerceApi.Settings;

namespace SmallEcommerceApi.Services
{
    public class JwtService
    {
        private readonly JwtSettings _settings;

        public JwtService(IOptions<JwtSettings> settings)
        {
            _settings = settings.Value;
        }

        public string GenerateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypesCustom.UserId, user.UserId.ToString()),
                new Claim(ClaimTypesCustom.Email, user.Email),
                new Claim(ClaimTypesCustom.Role, user.Role?.RoleName ?? "Customer")
            };

            if (string.IsNullOrEmpty(_settings.SecretKey))
            {
                throw new InvalidOperationException("JWT secret key is not configured.");
            }

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_settings.SecretKey));

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_settings.ExpireMinutes),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}