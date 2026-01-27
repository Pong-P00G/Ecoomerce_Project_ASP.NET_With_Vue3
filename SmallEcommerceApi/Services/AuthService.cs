using Microsoft.EntityFrameworkCore;
using SmallEcommerceApi.Db;
using SmallEcommerceApi.DTOs.Auth;
using SmallEcommerceApi.Models.Auth;
using SmallEcommerceApi.Models.Users;
using SmallEcommerceApi.Services.Interfaces;
using SmallEcommerceApi.Settings;
using static SmallEcommerceApi.DTOs.Auth.AuthDto;

namespace SmallEcommerceApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly JwtSettings _jwtSettings;

        public AuthService(
            AppDbContext context,
            IPasswordHasher passwordHasher,
            ITokenService tokenService,
            JwtSettings jwtSettings)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _jwtSettings = jwtSettings;
        }

        public async Task<AuthResponseDto?> RegisterAsync(RegisterDto registerDto)
        {
            // Check if username or email already exists
            if (await _context.Users.AnyAsync(u => u.Username == registerDto.Username || u.Email == registerDto.Email))
            {
                return null;
            }

            // Check if role exists
            var roleExists = await _context.UserRoles.AnyAsync(r => r.RoleId == registerDto.RoleId);
            if (!roleExists)
            {
                return null;
            }

            // Create user
            var user = new User
            {
                Username = registerDto.Username!,
                Email = registerDto.Email!,
                PasswordHash = _passwordHasher.HashPassword(registerDto.Password!),
                FirstName = registerDto.FirstName!,
                MidName = registerDto.MidName,
                LastName = registerDto.LastName!,
                RoleId = registerDto.RoleId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Create user profile (separate from user creation to avoid circular reference)
            var profile = new UserProfile
            {
                UserId = user.UserId,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.UserProfiles.Add(profile);
            await _context.SaveChangesAsync();

            // Load role for token generation
            await _context.Entry(user).Reference(u => u.Role).LoadAsync();

            return await GenerateAuthResponse(user);
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u =>
                    u.Username == loginDto.UsernameOrEmail ||
                    u.Email == loginDto.UsernameOrEmail);

            if (user == null || !_passwordHasher.VerifyPassword(loginDto.Password!, user.PasswordHash))
            {
                return null;
            }

            if (!user.IsActive)
            {
                return null;
            }

            // Update last login
            user.LastLogin = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return await GenerateAuthResponse(user);
        }   

        public async Task<AuthResponseDto?> RefreshTokenAsync(string refreshToken)
        {
            var storedToken = await _context.RefreshTokens
                .Include(rt => rt.User)
                .ThenInclude(u => u!.Role)
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (storedToken == null || storedToken.IsRevoked || storedToken.ExpiresAt < DateTime.UtcNow)
            {
                return null;
            }

            if (storedToken.User == null || !storedToken.User.IsActive)
            {
                return null;
            }

            // Revoke old token
            storedToken.IsRevoked = true;
            await _context.SaveChangesAsync();

            return await GenerateAuthResponse(storedToken.User);
        }

        public async Task<bool> LogoutAsync(int userId)
        {
            var tokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && !rt.IsRevoked)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.IsRevoked = true;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RevokeRefreshTokenAsync(string refreshToken)
        {
            var token = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (token == null)
            {
                return false;
            }

            token.IsRevoked = true;
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task<AuthResponseDto> GenerateAuthResponse(User user)
        {
            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.UserId,
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDays),
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.RefreshTokens.Add(refreshTokenEntity);
            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpireMinutes),
                User = new UserDto
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    MidName = user.MidName,
                    LastName = user.LastName,
                    FullName = user.FullName, 
                    RoleId = user.RoleId,
                    RoleName = user.Role?.RoleName,
                    IsActive = user.IsActive,
                    LastLogin = user.LastLogin,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                }
            };
        }
    }
}