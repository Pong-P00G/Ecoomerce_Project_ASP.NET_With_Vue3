using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using SmallEcommerceApi.Db;
using SmallEcommerceApi.DTOs.Auth;
using SmallEcommerceApi.Models.Auth;
using SmallEcommerceApi.Models.Users;
using SmallEcommerceApi.Security;
using SmallEcommerceApi.Security.Api.Security;
using SmallEcommerceApi.Services;
using SmallEcommerceApi.Services.Interfaces;
using static SmallEcommerceApi.DTOs.Auth.AuthDto;

namespace SmallEcommerceApi.Controllers.Auth
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IPasswordHasher _passwordService;
        private readonly JwtService _jwtService;
        private readonly IAuthService _authService;

        public AuthController(
            AppDbContext db,
            IPasswordHasher passwordService,
            JwtService jwtService,
            IAuthService authService)
        {
            _db = db;
            _passwordService = passwordService;
            _jwtService = jwtService;
            _authService = authService;
        }

        // REGISTER
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterDto registerDto)
        {
            if (string.IsNullOrWhiteSpace(registerDto.Password))
                return BadRequest("Password is required.");

            var result = await _authService.RegisterAsync(registerDto);

            if (result == null)
                return BadRequest("Registration failed. Email or username may already exist.");

            return Ok(result);
        }

        // LOGIN (JWT) - FIXED
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Password is required.");

            var result = await _authService.LoginAsync(dto);

            if (result == null)
                return BadRequest("Invalid username/email or password.");

            return Ok(result);
        }

        // REFRESH TOKEN - NEW
        [HttpPost("refresh")]
        public async Task<ActionResult<AuthResponseDto>> RefreshToken([FromBody] DTOs.Auth.RefreshTokenDto refreshTokenDto)
        {
            if (string.IsNullOrWhiteSpace(refreshTokenDto.RefreshToken))
                return BadRequest("Refresh token is required.");

            var result = await _authService.RefreshTokenAsync(refreshTokenDto.RefreshToken);

            if (result == null)
                return Unauthorized("Invalid or expired refresh token.");

            return Ok(result);
        }

        // LOGOUT - NEW
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userIdClaim = User.FindFirst(ClaimTypesCustom.UserId)?.Value;

            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }

            await _authService.LogoutAsync(userId);

            return Ok(new { message = "Logged out successfully." });
        }

        [Authorize]
        [HttpPost("revoke")]
        public async Task<IActionResult> RevokeToken([FromBody] DTOs.Auth.RefreshTokenDto refreshTokenDto)
        {
            if (string.IsNullOrWhiteSpace(refreshTokenDto.RefreshToken))
                return BadRequest("Refresh token is required.");

            var result = await _authService.RevokeRefreshTokenAsync(refreshTokenDto.RefreshToken);

            if (!result)
            {
                return BadRequest(new { message = "Token not found." });
            }

            return Ok(new { message = "Token revoked successfully." });
        }


        [Authorize]
        [HttpGet("me")]
        public IActionResult GetCurrentUser()
        {
            var userId = User.FindFirst(ClaimTypesCustom.UserId)?.Value;
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var email = User.FindFirst(ClaimTypesCustom.Email)?.Value;
            var role = User.FindFirst(ClaimTypesCustom.Role)?.Value;

            return Ok(new
            {
                userId,
                username,
                email,
                role
            });
        }
    }
}
