using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmallEcommerceApi.Db;
using SmallEcommerceApi.Models.Users;
using SmallEcommerceApi.Services.Interfaces;

namespace SmallEcommerceApi.Controllers.Common
{
    [ApiController]
    [Route("api/password")]
    public class PasswordController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IPasswordResetService _service;
        private readonly PasswordHasher<User> _hasher = new();

        public PasswordController(AppDbContext db, IPasswordResetService service)
        {
            _db = db;
            _service = service;
        }

        [HttpPost("forgot")]
        public async Task<IActionResult> Forgot(string email)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return Ok(); // prevent email enumeration

            var token = Guid.NewGuid().ToString();
            await _service.CreateTokenAsync(user.UserId, token, DateTime.UtcNow.AddMinutes(30));

            // send token via email in production
            return Ok(new { message = "Reset link sent", token });
        }

        [HttpPost("reset")]
        public async Task<IActionResult> Reset(string token, string newPassword)
        {
            var resetToken = await _service.GetValidTokenAsync(token);
            if (resetToken == null) return BadRequest("Invalid or expired token");

            var user = await _db.Users.FindAsync(resetToken.UserId);
            if (user == null) return BadRequest("User not found");

            user.PasswordHash = _hasher.HashPassword(user, newPassword);

            await _service.MarkAsUsedAsync(resetToken);
            await _db.SaveChangesAsync();

            return Ok("Password updated");
        }
    }

}
