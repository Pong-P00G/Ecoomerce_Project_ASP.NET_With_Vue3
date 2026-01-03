using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmallEcommerceApi.Db;
using SmallEcommerceApi.DTOs.Auth;
using SmallEcommerceApi.Models.Users;

namespace SmallEcommerceApi.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly AppDbContext _db;
        private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();

        public UserController(AppDbContext db)
        {
            _db = db;
        }

        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<List<UserDto>>> GetUsers()
        {
            var users = await _db.Users
                .Select(u => new UserDto
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    IsActive = u.IsActive
                })
                .ToListAsync();

            return Ok(users);
        }

        // GET: api/UserById
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserById(int id)
        {
            var user = await _db.Users.FindAsync(id);

            if (user == null)
                return NotFound($"User with ID {id} not found.");

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

        // PUT: api/user/updateUser
        [HttpPut("{id}")]
        public async Task<ActionResult<UserDto>> UpdateUser(int id, [FromBody] UpdateUserDto dto)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null)
                return NotFound($"User with ID {id} not found.");

            // Check if email is being updated and is already taken by another user
            if (!string.IsNullOrEmpty(dto.Email) &&
                await _db.Users.AnyAsync(u => u.Email == dto.Email && u.UserId != id))
            {
                return BadRequest("Email already exists.");
            }

            // Check if username is being updated and is already taken by another user
            if (!string.IsNullOrEmpty(dto.Username) &&
                await _db.Users.AnyAsync(u => u.Username == dto.Username && u.UserId != id))
            {
                return BadRequest("Username already exists.");
            }

            // Update fields
            if (!string.IsNullOrEmpty(dto.Username)) user.Username = dto.Username;
            if (!string.IsNullOrEmpty(dto.Email)) user.Email = dto.Email;
            if (!string.IsNullOrEmpty(dto.FirstName)) user.FirstName = dto.FirstName;
            if (!string.IsNullOrEmpty(dto.LastName)) user.LastName = dto.LastName;
            if (dto.IsActive.HasValue) user.IsActive = dto.IsActive.Value;

            // Update password if provided
            if (!string.IsNullOrEmpty(dto.Password))
            {
                user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);
            }

            await _db.SaveChangesAsync();

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


        // DELETE: api/user/1
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null)
                return NotFound($"User with ID {id} not found.");

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();

            return Ok($"User with ID {id} has been deleted.");
        }

    }
}
