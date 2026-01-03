using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmallEcommerceApi.Db;
using SmallEcommerceApi.DTOs.Auth;
using SmallEcommerceApi.Mapping;
using SmallEcommerceApi.Models.Users;
using static SmallEcommerceApi.DTOs.Auth.Login;

namespace SmallEcommerceApi.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();

        public AuthController(AppDbContext db)
        {
            _db = db;
        }

        // REGISTER
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto dto)
        {
            if (!RoleMap.ContainsKey(dto.Role))
                return BadRequest("Invalid role.");

            if (dto.Role.Equals("admin", StringComparison.OrdinalIgnoreCase))
                return Forbid("Admin registration is not allowed.");

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = _passwordHasher.HashPassword(null, dto.Password),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                RoleId = RoleMap.GetRoleId(dto.Role),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return Ok(new UserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                IsActive = user.IsActive
            });
        }

        // LOGIN
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login([FromBody] LoginDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u =>
                u.Email.Equals(dto.Email, StringComparison.OrdinalIgnoreCase));

            if (user == null)
                return BadRequest("Invalid email or password.");

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (result == PasswordVerificationResult.Failed)
                return BadRequest("Invalid email or password.");

            var userDto = new UserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsActive = user.IsActive
            };

            return Ok(userDto);
        }

        // FORGOT PASSWORD
        [HttpPost("forgot-password")]
        public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u =>
                u.Email.Equals(dto.Email, StringComparison.OrdinalIgnoreCase));

            if (user == null)
                return NotFound("Email not found.");

            // TODO: send reset link via email in production
            return Ok($"Password reset link sent to {dto.Email} (simulated).");
        }
    }
}
