using SmallEcommerceApi.DTOs.Auth;

namespace SmallEcommerceApi.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto?> GetUserByIdAsync(int userId);
        Task<UserDto?> CreateUserAsync(CreateUserDto createUserDto);
        Task<UserDto?> UpdateUserAsync(int userId, UpdateUserDto updateUserDto);
        Task<bool> DeleteUserAsync(int userId);
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto);
        Task<UserProfileDto?> GetUserProfileAsync(int userId);
        Task<UserProfileDto?> UpdateUserProfileAsync(int userId, UpdateUserProfileDto updateProfileDto);
    }
}