using Microsoft.EntityFrameworkCore;
using SmallEcommerceApi.Db;
using SmallEcommerceApi.DTOs;
using SmallEcommerceApi.Models;
using SmallEcommerceApi.Models.Users;
using SmallEcommerceApi.DTOs.Auth;
using SmallEcommerceApi.Services.Interfaces;

namespace SmallEcommerceApi.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(AppDbContext context, IPasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            return await _context.Users
                .Include(u => u.Role)
                .Select(u => new UserDto
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    MidName = u.MidName,
                    LastName = u.LastName,
                    FullName = u.FullName,
                    RoleId = u.RoleId,
                    RoleName = u.Role!.RoleName,
                    IsActive = u.IsActive,
                    LastLogin = u.LastLogin,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<UserDto?> GetUserByIdAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                return null;

            return new UserDto
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
            };
        }

        public async Task<UserDto?> CreateUserAsync(CreateUserDto createUserDto)
        {
            // Check if username or email already exists
            if (await _context.Users.AnyAsync(u => u.Username == createUserDto.Username || u.Email == createUserDto.Email))
            {
                return null;
            }

            // Check if role exists
            var roleExists = await _context.UserRoles.AnyAsync(r => r.RoleId == createUserDto.RoleId);
            if (!roleExists)
            {
                return null;
            }

            var user = new User
            {
                Username = createUserDto.Username,
                Email = createUserDto.Email,
                PasswordHash = _passwordHasher.HashPassword(createUserDto.Password),
                FirstName = createUserDto.FirstName,
                MidName = createUserDto.MidName,
                LastName = createUserDto.LastName,
                RoleId = createUserDto.RoleId,
                IsActive = createUserDto.IsActive,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Create default profile
            var profile = new UserProfile
            {
                UserId = user.UserId,
                FirstName = createUserDto.FirstName,
                LastName = createUserDto.LastName,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.UserProfiles.Add(profile);
            await _context.SaveChangesAsync();

            return await GetUserByIdAsync(user.UserId);
        }

        public async Task<UserDto?> UpdateUserAsync(int userId, UpdateUserDto updateUserDto)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return null;

            // Check for duplicate username/email if being changed
            if (!string.IsNullOrWhiteSpace(updateUserDto.Username) && updateUserDto.Username != user.Username)
            {
                if (await _context.Users.AnyAsync(u => u.Username == updateUserDto.Username))
                    return null;
                user.Username = updateUserDto.Username;
            }

            if (!string.IsNullOrWhiteSpace(updateUserDto.Email) && updateUserDto.Email != user.Email)
            {
                if (await _context.Users.AnyAsync(u => u.Email == updateUserDto.Email))
                    return null;
                user.Email = updateUserDto.Email;
            }

            if (!string.IsNullOrWhiteSpace(updateUserDto.FirstName))
                user.FirstName = updateUserDto.FirstName;

            if (updateUserDto.MidName != null)
                user.MidName = updateUserDto.MidName;

            if (!string.IsNullOrWhiteSpace(updateUserDto.LastName))
                user.LastName = updateUserDto.LastName;

            if (updateUserDto.RoleId.HasValue)
            {
                var roleExists = await _context.UserRoles.AnyAsync(r => r.RoleId == updateUserDto.RoleId.Value);
                if (!roleExists)
                    return null;
                user.RoleId = updateUserDto.RoleId.Value;
            }

            if (updateUserDto.IsActive.HasValue)
                user.IsActive = updateUserDto.IsActive.Value;

            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return await GetUserByIdAsync(userId);
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return false;

            if (!_passwordHasher.VerifyPassword(changePasswordDto.CurrentPassword, user.PasswordHash))
                return false;

            user.PasswordHash = _passwordHasher.HashPassword(changePasswordDto.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<UserProfileDto?> GetUserProfileAsync(int userId)
        {
            var profile = await _context.UserProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile == null)
                return null;

            return new UserProfileDto
            {
                ProfileId = profile.ProfileId,
                UserId = profile.UserId,
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                AvatarUrl = profile.AvatarUrl,
                Phone = profile.Phone,
                DateOfBirth = profile.DateOfBirth,
                Gender = profile.Gender,
                Bio = profile.Bio
            };
        }

        public async Task<UserProfileDto?> UpdateUserProfileAsync(int userId, UpdateUserProfileDto updateProfileDto)
        {
            var profile = await _context.UserProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (profile == null)
            {
                // Create profile if it doesn't exist
                profile = new UserProfile
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };
                _context.UserProfiles.Add(profile);
            }

            if (!string.IsNullOrWhiteSpace(updateProfileDto.FirstName))
                profile.FirstName = updateProfileDto.FirstName;

            if (!string.IsNullOrWhiteSpace(updateProfileDto.LastName))
                profile.LastName = updateProfileDto.LastName;

            if (updateProfileDto.AvatarUrl != null)
                profile.AvatarUrl = updateProfileDto.AvatarUrl;

            if (updateProfileDto.Phone != null)
                profile.Phone = updateProfileDto.Phone;

            if (updateProfileDto.DateOfBirth.HasValue)
                profile.DateOfBirth = updateProfileDto.DateOfBirth;

            if (!string.IsNullOrWhiteSpace(updateProfileDto.Gender))
                profile.Gender = updateProfileDto.Gender;

            if (updateProfileDto.Bio != null)
                profile.Bio = updateProfileDto.Bio;

            profile.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return await GetUserProfileAsync(userId);
        }
    }
}