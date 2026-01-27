using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using SmallEcommerceApi.Db;
using SmallEcommerceApi.Security.Api.Security;
using SmallEcommerceApi.DTOs.Auth;
using SmallEcommerceApi.Services.Interfaces;

namespace SmallEcommerceApi.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // Get all user
        [HttpGet]
        [Authorize(Roles = "ADMIN,Admin,admin")] // role id 1 is admin
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        // Get user by id
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetUserById(int id)
        {
            // Users can only view their own profile unless they're admin
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (currentUserId != id && !string.Equals(currentUserRole, "ADMIN", StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }

            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            return Ok(user);
        }

        // Create new user(Admin only)
        [HttpPost]
        [Authorize(Roles = "ADMIN,Admin,admin")]
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            var user = await _userService.CreateUserAsync(createUserDto);

            if (user == null)
            {
                return BadRequest(new { message = "Username or email already exists, or invalid role." });
            }

            return CreatedAtAction(nameof(GetUserById), new { id = user.UserId }, user);
        }

        // Update user information
        [HttpPut("{id}")]
        public async Task<ActionResult<UserDto>> UpdateUser(int id, [FromBody] UpdateUserDto updateUserDto)
        {
            // Users can only update their own profile unless they're admin
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (currentUserId != id && !string.Equals(currentUserRole, "ADMIN", StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }

            // Non-admin users cannot change role
            if (!string.Equals(currentUserRole, "ADMIN", StringComparison.OrdinalIgnoreCase) && updateUserDto.RoleId.HasValue)
            {
                return BadRequest(new { message = "Only ADMIN users can change roles." });
            }

            var user = await _userService.UpdateUserAsync(id, updateUserDto);

            if (user == null)
            {
                return NotFound(new { message = "User not found or username/email already exists." });
            }

            return Ok(user);
        }

        // Change user password
        [HttpPost("{id}/change-password")]
        public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordDto changePasswordDto)
        {
            // Users can only change their own password
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            if (currentUserId != id)
            {
                return Forbid();
            }

            var result = await _userService.ChangePasswordAsync(id, changePasswordDto);

            if (!result)
            {
                return BadRequest(new { message = "User not found or current password is incorrect." });
            }

            return Ok(new { message = "Password changed successfully." });
        }

        // Get user profile
        [HttpGet("{id}/profile")]
        public async Task<ActionResult<UserProfileDto>> GetUserProfile(int id)
        {
            // Users can only view their own profile unless they're admin
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (currentUserId != id && !string.Equals(currentUserRole, "ADMIN", StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }

            var profile = await _userService.GetUserProfileAsync(id);

            if (profile == null)
            {
                return NotFound(new { message = "Profile not found." });
            }

            return Ok(profile);
        }

        // Update user profile
        [HttpPut("{id}/profile")]
        public async Task<ActionResult<UserProfileDto>> UpdateUserProfile(int id, [FromBody] UpdateUserProfileDto updateProfileDto)
        {
            // Users can only update their own profile
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            if (currentUserId != id)
            {
                return Forbid();
            }

            var profile = await _userService.UpdateUserProfileAsync(id, updateProfileDto);

            if (profile == null)
            {
                return NotFound(new { message = "User not found." });
            }

            return Ok(profile);
        }
        // Delete user (Admin only)
        [HttpDelete("{id}")]
        [Authorize(Roles = "ADMIN,Admin,admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            if (currentUserId == id)
            {
                return BadRequest(new { message = "You cannot delete your own account." });
            }

            var result = await _userService.DeleteUserAsync(id);

            if (!result)
            {
                return NotFound(new { message = "User not found." });
            }

            return Ok(new { message = "User deleted successfully." });
        }
    }
}
